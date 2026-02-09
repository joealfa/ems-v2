namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a recent activity log entry.
/// This is an immutable log record that does not support soft deletes or audit trails.
/// </summary>
public class RecentActivity
{
    /// <summary>
    /// Gets or sets the unique identifier for the activity.
    /// Auto-increment primary key.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the type of entity involved in the activity.
    /// Examples: "person", "school", "item", etc.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display ID or identifier of the entity.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operation performed.
    /// Examples: "CREATE", "UPDATE", "DELETE", etc.
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the human-readable message describing the activity.
    /// Example: "Person 'John Doe' was created"
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the activity occurred (UTC).
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the person who performed the action.
    /// </summary>
    public string? UserId { get; set; }
}
