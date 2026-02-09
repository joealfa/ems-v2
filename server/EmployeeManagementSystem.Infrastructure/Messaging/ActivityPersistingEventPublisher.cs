using EmployeeManagementSystem.Application.Events;
using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Events;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EmployeeManagementSystem.Infrastructure.Messaging;

/// <summary>
/// Decorator for IEventPublisher that persists activities to the database before publishing to RabbitMQ.
/// Follows the Decorator pattern to add persistence behavior to event publishing.
/// </summary>
public class ActivityPersistingEventPublisher : IEventPublisher
{
    private readonly IEventPublisher _innerPublisher;
    private readonly IRecentActivityRepository _activityRepository;
    private readonly ILogger<ActivityPersistingEventPublisher> _logger;

    public ActivityPersistingEventPublisher(
        IEventPublisher innerPublisher,
        IRecentActivityRepository activityRepository,
        ILogger<ActivityPersistingEventPublisher> logger)
    {
        _innerPublisher = innerPublisher;
        _activityRepository = activityRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(
        TEvent domainEvent,
        string? userId = null,
        string? correlationId = null,
        EventMetadata? metadata = null,
        CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        // First, try to persist the activity to the database
        try
        {
            RecentActivity activity = CreateRecentActivity(domainEvent, userId);
            await _activityRepository.AddAsync(activity, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the entire event publishing
            _logger.LogError(ex, "Failed to persist activity for event {EventType}", domainEvent.EventType);
        }

        // Then, delegate to the inner publisher (RabbitMQ)
        await _innerPublisher.PublishAsync(domainEvent, userId, correlationId, metadata, cancellationToken);
    }

    /// <inheritdoc />
    public async Task PublishBatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        string? userId = null,
        string? correlationId = null,
        EventMetadata? metadata = null,
        CancellationToken cancellationToken = default)
    {
        // Persist all activities in batch
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            try
            {
                RecentActivity activity = CreateRecentActivity(domainEvent, userId);
                await _activityRepository.AddAsync(activity, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the entire event publishing
                _logger.LogError(ex, "Failed to persist activity for event {EventType}", domainEvent.EventType);
            }
        }

        // Then, delegate to the inner publisher (RabbitMQ)
        await _innerPublisher.PublishBatchAsync(domainEvents, userId, correlationId, metadata, cancellationToken);
    }

    /// <summary>
    /// Creates a RecentActivity entity from a domain event.
    /// </summary>
    private RecentActivity CreateRecentActivity<TEvent>(TEvent domainEvent, string? userId)
        where TEvent : IDomainEvent
    {
        string entityType = GetEntityType(domainEvent);
        string entityId = GetEntityId(domainEvent);
        string operation = GetOperation(domainEvent);
        string message = GenerateFriendlyMessage(entityType, operation, domainEvent);

        return new RecentActivity
        {
            EntityType = entityType,
            EntityId = entityId,
            Operation = operation,
            Message = message,
            Timestamp = domainEvent.OccurredOn,
            UserId = userId
        };
    }

    /// <summary>
    /// Extracts the entity type from the domain event.
    /// Event type format: "com.ems.{entity}.{operation}"
    /// </summary>
    private static string GetEntityType(IDomainEvent domainEvent)
    {
        string[] parts = domainEvent.EventType.Split('.');
        return parts.Length >= 3 ? parts[2] : "unknown";
    }

    /// <summary>
    /// Extracts the entity ID from the domain event using reflection.
    /// </summary>
    private static string GetEntityId(IDomainEvent domainEvent)
    {
        PropertyInfo? idProperty = domainEvent.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.EndsWith("Id") && !p.Name.Equals("EventId"));

        return idProperty?.GetValue(domainEvent)?.ToString() ?? "unknown";
    }

    /// <summary>
    /// Extracts the operation from the domain event.
    /// </summary>
    private static string GetOperation(IDomainEvent domainEvent)
    {
        string[] parts = domainEvent.EventType.Split('.');
        string lastPart = parts[^1].ToUpperInvariant();

        return lastPart.Contains("CREATED")
            ? "CREATE"
            : lastPart.Contains("UPDATED")
            ? "UPDATE"
            : lastPart.Contains("DELETED")
            ? "DELETE"
            : lastPart.Contains("ASSIGNED")
            ? "ASSIGN"
            : lastPart.Contains("REMOVED")
            ? "REMOVE"
            : lastPart.Contains("UPLOADED") ? "UPLOAD" : "UNKNOWN";
    }

    /// <summary>
    /// Generates a friendly human-readable message for the activity.
    /// Pattern follows Gateway's GenerateFriendlyMessage logic.
    /// </summary>
    private static string GenerateFriendlyMessage<TEvent>(string entityType, string operation, TEvent domainEvent)
        where TEvent : IDomainEvent
    {
        string entityName = GetEntityDisplayName(entityType);
        string actionVerb = GetActionVerb(operation);
        string? identifier = GetEntityIdentifier(entityType, domainEvent);

        return !string.IsNullOrEmpty(identifier)
            ? $"{entityName} '{identifier}' was {actionVerb}"
            : $"A {entityName.ToLower()} was {actionVerb}";
    }

    /// <summary>
    /// Gets a human-readable display name for the entity type.
    /// </summary>
    private static string GetEntityDisplayName(string entityType)
    {
        return entityType.ToLowerInvariant() switch
        {
            "person" => "Person",
            "school" => "School",
            "employment" => "Employment",
            "item" => "Item",
            "position" => "Position",
            "salarygrade" => "Salary Grade",
            "blob" => "File",
            _ => entityType
        };
    }

    /// <summary>
    /// Gets a past-tense verb for the operation.
    /// </summary>
    private static string GetActionVerb(string operation)
    {
        return operation.ToUpperInvariant() switch
        {
            "CREATE" => "created",
            "UPDATE" => "updated",
            "DELETE" => "deleted",
            "ASSIGN" => "assigned",
            "REMOVE" => "removed",
            "UPLOAD" => "uploaded",
            _ => operation.ToLower()
        };
    }

    /// <summary>
    /// Extracts a human-readable identifier for the entity from the domain event.
    /// </summary>
    private static string? GetEntityIdentifier<TEvent>(string entityType, TEvent domainEvent)
        where TEvent : IDomainEvent
    {
        try
        {
            PropertyInfo[] properties = domainEvent.GetType().GetProperties();

            return entityType.ToLowerInvariant() switch
            {
                "person" => GetPersonIdentifier(properties, domainEvent),
                "school" => GetPropertyValue(properties, domainEvent, "SchoolName"),
                "item" => GetPropertyValue(properties, domainEvent, "Name"),
                "position" => GetPropertyValue(properties, domainEvent, "PositionTitle"),
                "salarygrade" => GetPropertyValue(properties, domainEvent, "Grade"),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets a person identifier (full name or DisplayId).
    /// </summary>
    private static string? GetPersonIdentifier<TEvent>(PropertyInfo[] properties, TEvent domainEvent)
        where TEvent : IDomainEvent
    {
        string? firstName = GetPropertyValue(properties, domainEvent, "FirstName");
        string? lastName = GetPropertyValue(properties, domainEvent, "LastName");

        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
        {
            return $"{firstName} {lastName}";
        }

        return GetPropertyValue(properties, domainEvent, "DisplayId");
    }

    /// <summary>
    /// Gets a property value from the domain event using reflection.
    /// </summary>
    private static string? GetPropertyValue<TEvent>(PropertyInfo[] properties, TEvent domainEvent, string propertyName)
        where TEvent : IDomainEvent
    {
        PropertyInfo? property = properties.FirstOrDefault(p =>
            p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        return property?.GetValue(domainEvent)?.ToString();
    }
}
