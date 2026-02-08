using EmployeeManagementSystem.Gateway.Caching;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EmployeeManagementSystem.Gateway.Messaging;

/// <summary>
/// RabbitMQ event consumer that listens for domain events and invalidates cache.
/// </summary>
public sealed class RabbitMQEventConsumer : IDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMQEventConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ResiliencePipeline? _retryPipeline;
    private bool _disposed;

    private static readonly JsonSerializerOptions PrettyPrintOptions = new()
    {
        WriteIndented = true
    };

    public RabbitMQEventConsumer(
        IOptions<RabbitMQSettings> settings,
        IServiceProvider serviceProvider,
        ILogger<RabbitMQEventConsumer> logger)
    {
        _settings = settings.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;

        if (_settings.Enabled)
        {
            _retryPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = _settings.RetryCount,
                    Delay = TimeSpan.FromMilliseconds(_settings.RetryDelayMilliseconds),
                    BackoffType = DelayBackoffType.Exponential,
                    OnRetry = args =>
                    {
                        _logger.LogWarning("Retry attempt {Attempt} for RabbitMQ operation", args.AttemptNumber);
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        }
    }

    /// <summary>
    /// Initializes the RabbitMQ connection and starts consuming events.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("RabbitMQ event consumer is disabled");
            return;
        }

        try
        {
            await _retryPipeline!.ExecuteAsync(async ct =>
            {
                InitializeConnection();
                await Task.CompletedTask;
            }, cancellationToken);

            _logger.LogInformation("RabbitMQ event consumer started successfully. Listening on queue: {QueueName}", _settings.QueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start RabbitMQ event consumer");
            throw;
        }
    }

    /// <summary>
    /// Stops the RabbitMQ consumer.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ event consumer");

        try
        {
            _channel?.Close();
            _connection?.Close();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while stopping RabbitMQ consumer");
        }

        return Task.CompletedTask;
    }

    private void InitializeConnection()
    {
        ConnectionFactory factory = new()
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.UserName,
            Password = _settings.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        if (_settings.UseSsl)
        {
            factory.Ssl = new SslOption
            {
                Enabled = true,
                ServerName = _settings.HostName
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

        // Declare queue
        _ = _channel.QueueDeclare(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                { "x-message-ttl", 86400000 }, // 24 hours
                { "x-max-length", 10000 }
            });

        // Bind queue to exchange with wildcard routing key for all EMS events
        _channel.QueueBind(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: "com.ems.#");

        // Set up consumer
        EventingBasicConsumer consumer = new(_channel);
        consumer.Received += async (model, ea) =>
        {
            await HandleMessageAsync(ea);
        };

        _ = _channel.BasicConsume(
            queue: _settings.QueueName,
            autoAck: true, // Auto-acknowledge for simplicity
            consumer: consumer);

        _logger.LogInformation("RabbitMQ connection established. Queue: {QueueName}, Exchange: {ExchangeName}",
            _settings.QueueName, _settings.ExchangeName);
    }

    private async Task HandleMessageAsync(BasicDeliverEventArgs ea)
    {
        try
        {
            string body = Encoding.UTF8.GetString(ea.Body.ToArray());
            string routingKey = ea.RoutingKey;

            // Print to console with clear formatting
            Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("║ 📨 RABBITMQ MESSAGE RECEIVED");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine($"║ Routing Key: {routingKey}");
            Console.WriteLine($"║ Timestamp:    {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("║ MESSAGE BODY:");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════════");

            // Pretty print the JSON message
            try
            {
                JsonDocument jsonDoc = JsonDocument.Parse(body);
                string prettyJson = JsonSerializer.Serialize(jsonDoc, PrettyPrintOptions);

                foreach (string line in prettyJson.Split('\n'))
                {
                    Console.WriteLine($"║ {line}");
                }
            }
            catch
            {
                // If JSON parsing fails, just print raw body
                Console.WriteLine($"║ {body}");
            }

            Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            _logger.LogDebug("Received event with routing key: {RoutingKey}", routingKey);

            // Deserialize CloudEvent
            CloudEvent<EventMessage>? cloudEvent = JsonSerializer.Deserialize<CloudEvent<EventMessage>>(body);

            if (cloudEvent?.Data == null)
            {
                _logger.LogWarning("Received invalid CloudEvent format");
                return;
            }

            // Invalidate cache based on event type
            await InvalidateCacheForEventAsync(cloudEvent.Type, cloudEvent.Data);

            _logger.LogInformation("✅ Processed event {EventType} for {EntityType}:{EntityId}",
                cloudEvent.Type, cloudEvent.Data.EntityType, cloudEvent.Data.EntityId);

            Console.WriteLine($"✅ Cache invalidated for event: {cloudEvent.Type}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RabbitMQ message with routing key {RoutingKey}", ea.RoutingKey);
            Console.WriteLine($"❌ ERROR processing message: {ex.Message}");
            Console.WriteLine();
            // Message is already ack'd, so it won't be redelivered
        }
    }

    private async Task InvalidateCacheForEventAsync(string eventType, EventMessage eventData)
    {
        // Extract entity type from event type (e.g., "com.ems.person.created" -> "person")
        string[] parts = eventType.Split('.');
        if (parts.Length < 3)
        {
            _logger.LogWarning("Invalid event type format: {EventType}", eventType);
            return;
        }

        string entityType = parts[2]; // person, school, item, etc.
        string operation = parts.Length > 3 ? parts[3] : "unknown"; // created, updated, deleted

        _logger.LogDebug("Invalidating cache for {EntityType} operation {Operation}", entityType, operation);

        // Create a scope to get the scoped cache service
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRedisCacheService cacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();

        switch (entityType.ToLowerInvariant())
        {
            case "person":
                await InvalidatePersonCacheAsync(cacheService, eventData, operation);
                break;

            case "school":
                await InvalidateSchoolCacheAsync(cacheService, eventData, operation);
                break;

            case "item":
                await InvalidateItemCacheAsync(cacheService, eventData, operation);
                break;

            case "position":
                await InvalidatePositionCacheAsync(cacheService, eventData, operation);
                break;

            case "salarygrade":
                await InvalidateSalaryGradeCacheAsync(cacheService, eventData, operation);
                break;

            case "employee":
                await InvalidateEmploymentCacheAsync(cacheService, eventData, operation);
                break;

            case "blob":
                await InvalidateBlobRelatedCacheAsync(cacheService, eventData, operation);
                break;

            default:
                _logger.LogWarning("Unknown entity type for cache invalidation: {EntityType}", entityType);
                break;
        }

        // Always invalidate dashboard stats when any entity changes
        await cacheService.RemoveAsync(CacheKeys.DashboardStats);
    }

    private async Task InvalidatePersonCacheAsync(IRedisCacheService cacheService, EventMessage eventData, string operation)
    {
        // Parse displayId from entityId (format: Guid)
        // For detail cache, we would need to track Guid -> DisplayId mapping
        // For now, invalidate all person lists
        await cacheService.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix);

        // Also invalidate employment lists since they reference person data
        await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);

        _logger.LogDebug("Invalidated person and employment list caches");
    }

    private async Task InvalidateSchoolCacheAsync(IRedisCacheService cacheService, EventMessage eventData, string operation)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.SchoolsListPrefix);
        await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);

        _logger.LogDebug("Invalidated school and employment list caches");
    }

    private async Task InvalidateItemCacheAsync(IRedisCacheService cacheService, EventMessage eventData, string operation)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.ItemsListPrefix);

        _logger.LogDebug("Invalidated item list caches");
    }

    private async Task InvalidatePositionCacheAsync(IRedisCacheService cacheService, EventMessage eventData, string operation)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.PositionsListPrefix);
        await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);

        _logger.LogDebug("Invalidated position and employment list caches");
    }

    private async Task InvalidateSalaryGradeCacheAsync(IRedisCacheService cacheService, EventMessage eventData, string operation)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.SalaryGradesListPrefix);
        await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);

        _logger.LogDebug("Invalidated salary grade and employment list caches");
    }

    private async Task InvalidateEmploymentCacheAsync(IRedisCacheService cacheService, EventMessage eventData, string operation)
    {
        await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);

        _logger.LogDebug("Invalidated employment list caches");
    }

    private async Task InvalidateBlobRelatedCacheAsync(IRedisCacheService cacheService, EventMessage eventData, string operation)
    {
        // Blob events include relatedEntityType and relatedEntityId in metadata
        // For profile images, invalidate person caches
        if (eventData.Metadata?.TryGetValue("relatedEntityType", out string? entityType) == true)
        {
            if (entityType == "Person")
            {
                await cacheService.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix);
                await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);
                _logger.LogDebug("Invalidated person caches due to blob change");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _channel?.Dispose();
        _connection?.Dispose();
        _disposed = true;
    }
}
