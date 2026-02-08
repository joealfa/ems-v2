namespace EmployeeManagementSystem.Domain.Events;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// When the event occurred
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// CloudEvents specification type (e.g., "com.ems.person.created")
    /// </summary>
    string EventType { get; }
}
