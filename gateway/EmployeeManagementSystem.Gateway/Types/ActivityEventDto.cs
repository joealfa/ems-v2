namespace EmployeeManagementSystem.Gateway.Types;

/// <summary>
/// Represents a recent activity event for real-time updates.
/// </summary>
public record ActivityEventDto
{
    /// <summary>
    /// Gets the unique identifier for this event.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the CloudEvent type (e.g., 'com.ems.person.created').
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// Gets the entity type (e.g., 'person', 'school', 'employment').
    /// </summary>
    public required string EntityType { get; init; }

    /// <summary>
    /// Gets the entity ID as a string (supports both Guid and int IDs).
    /// </summary>
    public required string EntityId { get; init; }

    /// <summary>
    /// Gets the operation performed (CREATE, UPDATE, DELETE, etc.).
    /// </summary>
    public required string Operation { get; init; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the user ID who performed the action, if available.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Gets a user-friendly message describing the activity.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets additional metadata as key-value pairs.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
