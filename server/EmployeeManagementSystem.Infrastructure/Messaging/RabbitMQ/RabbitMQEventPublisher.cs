using EmployeeManagementSystem.Application.Events;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmployeeManagementSystem.Infrastructure.Messaging.RabbitMQ;

public sealed class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ResiliencePipeline? _retryPipeline;

    public RabbitMQEventPublisher(
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQEventPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        if (!_settings.Enabled)
        {
            _logger.LogWarning("RabbitMQ event publishing is disabled");
            return;
        }

        try
        {
            ConnectionFactory factory = new()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                VirtualHost = _settings.VirtualHost,
                UserName = _settings.UserName,
                Password = _settings.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };

            if (_settings.UseSsl)
            {
                factory.Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = _settings.HostName,
                    Version = SslProtocols.Tls13 | SslProtocols.Tls12
                };
            }

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange (idempotent)
            _channel.ExchangeDeclare(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            _logger.LogInformation("RabbitMQ connection established successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }

        // Create retry pipeline using Polly v8
        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = _settings.RetryCount,
                Delay = TimeSpan.FromMilliseconds(_settings.RetryDelayMilliseconds),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    _logger.LogWarning(
                        "Retry {Attempt} after {Delay}ms due to {Exception}",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public async Task PublishAsync<TEvent>(
        TEvent domainEvent,
        string? userId = null,
        string? correlationId = null,
        EventMetadata? metadata = null,
        CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        if (!_settings.Enabled || _channel == null)
        {
            _logger.LogDebug("RabbitMQ publishing disabled, skipping event {EventType}", domainEvent.EventType);
            return;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            CloudEvent<EventMessage> cloudEvent = CreateCloudEvent(domainEvent, userId, correlationId, metadata);
            string routingKey = domainEvent.EventType;

            await _retryPipeline.ExecuteAsync(async ct =>
            {
                PublishEvent(cloudEvent, routingKey);
                await Task.CompletedTask;
            }, cancellationToken);

            _logger.LogInformation(
                "Published event {EventType} with ID {EventId}",
                domainEvent.EventType,
                domainEvent.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to publish event {EventType} with ID {EventId}",
                domainEvent.EventType,
                domainEvent.EventId);
            throw;
        }
        finally
        {
            _ = _semaphore.Release();
        }
    }

    public async Task PublishBatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        string? userId = null,
        string? correlationId = null,
        EventMetadata? metadata = null,
        CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled || _channel == null)
        {
            _logger.LogDebug("RabbitMQ publishing disabled, skipping batch events");
            return;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            IBasicPublishBatch batch = _channel.CreateBasicPublishBatch();

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                CloudEvent<EventMessage> cloudEvent = CreateCloudEvent(domainEvent, userId, correlationId, metadata);
                string routingKey = domainEvent.EventType;
                ReadOnlyMemory<byte> body = SerializeEvent(cloudEvent);

                batch.Add(
                    exchange: _settings.ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    properties: CreateMessageProperties(),
                    body: body);
            }

            batch.Publish();

            _logger.LogInformation(
                "Published batch of {Count} events",
                domainEvents.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event batch");
            throw;
        }
        finally
        {
            _ = _semaphore.Release();
        }
    }

    private CloudEvent<EventMessage> CreateCloudEvent(
        IDomainEvent domainEvent,
        string? userId,
        string? correlationId,
        EventMetadata? metadata)
    {
        EventMessage eventMessage = new()
        {
            EntityType = GetEntityType(domainEvent),
            EntityId = GetEntityId(domainEvent),
            Operation = GetOperation(domainEvent),
            Timestamp = domainEvent.OccurredOn.ToString("O"),
            UserId = userId,
            CorrelationId = correlationId,
            Payload = GetPayload(domainEvent),
            Changes = GetChanges(domainEvent),
            Metadata = metadata
        };

        return new CloudEvent<EventMessage>
        {
            Type = domainEvent.EventType,
            Id = domainEvent.EventId.ToString(),
            Time = domainEvent.OccurredOn.ToString("O"),
            Data = eventMessage
        };
    }

    private void PublishEvent(
        CloudEvent<EventMessage> cloudEvent,
        string routingKey)
    {
        if (_channel == null)
        {
            return;
        }

        ReadOnlyMemory<byte> body = SerializeEvent(cloudEvent);
        IBasicProperties properties = CreateMessageProperties();

        _channel.BasicPublish(
            exchange: _settings.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }

    private ReadOnlyMemory<byte> SerializeEvent(CloudEvent<EventMessage> cloudEvent)
    {
        string json = JsonSerializer.Serialize(cloudEvent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        return Encoding.UTF8.GetBytes(json);
    }

    private IBasicProperties CreateMessageProperties()
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized");
        }

        IBasicProperties properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.ContentEncoding = "utf-8";
        properties.DeliveryMode = 2; // Persistent
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return properties;
    }

    // Helper methods to extract entity information from domain events
    private static string GetEntityType(IDomainEvent domainEvent)
    {
        string[] parts = domainEvent.EventType.Split('.');
        return parts.Length >= 3 ? parts[2] : "unknown";
    }

    private static string GetEntityId(IDomainEvent domainEvent)
    {
        // Use reflection to get the entity ID from the event
        PropertyInfo? idProperty = domainEvent.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.EndsWith("Id") && !p.Name.Equals("EventId"));

        return idProperty?.GetValue(domainEvent)?.ToString() ?? "unknown";
    }

    private static string GetOperation(IDomainEvent domainEvent)
    {
        string[] parts = domainEvent.EventType.Split('.');
        string lastPart = parts[^1].ToUpperInvariant();

        if (lastPart.Contains("CREATED"))
        {
            return "CREATE";
        }

        if (lastPart.Contains("UPDATED"))
        {
            return "UPDATE";
        }

        return lastPart.Contains("DELETED")
            ? "DELETE"
            : lastPart.Contains("ASSIGNED")
            ? "ASSIGN"
            : lastPart.Contains("REMOVED") ? "REMOVE" : lastPart.Contains("UPLOADED") ? "UPLOAD" : "UNKNOWN";
    }

    private static object? GetPayload(IDomainEvent domainEvent)
    {
        // Convert the domain event to a dictionary
        IEnumerable<PropertyInfo> properties = domainEvent.GetType().GetProperties()
            .Where(p => !p.Name.Equals("EventId") && !p.Name.Equals("OccurredOn") && !p.Name.Equals("EventType"));

        Dictionary<string, object?> payload = [];
        foreach (PropertyInfo? prop in properties)
        {
            payload[prop.Name] = prop.GetValue(domainEvent);
        }

        return payload;
    }

    private static List<FieldChange>? GetChanges(IDomainEvent domainEvent)
    {
        // Check if the event has a Changes property
        PropertyInfo? changesProperty = domainEvent.GetType().GetProperty("Changes");
        return changesProperty?.GetValue(domainEvent) is Dictionary<string, object?> changes
            ? changes.Select(kvp => new FieldChange
            {
                Field = kvp.Key,
                OldValue = null, // We don't track old values in this simple implementation
                NewValue = kvp.Value
            }).ToList()
            : null;
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _semaphore?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while disposing RabbitMQ connection");
        }
    }
}
