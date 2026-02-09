namespace EmployeeManagementSystem.Application.DTOs;

/// <summary>
/// Record representing a person with a birthday this month.
/// </summary>
public record BirthdayCelebrantDto
{
    /// <summary>
    /// Gets the display ID of the person.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets the first name of the person.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the last name of the person.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the middle name of the person.
    /// </summary>
    public string? MiddleName { get; init; }

    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the date of birth of the person.
    /// </summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets the profile image URL (will be set by Gateway).
    /// </summary>
    public string? ProfileImageUrl { get; init; }

    /// <summary>
    /// Gets whether the person has a profile image.
    /// </summary>
    public bool HasProfileImage { get; init; }
}

/// <summary>
/// Record representing a recent activity entry.
/// </summary>
public record RecentActivityDto
{
    /// <summary>
    /// Gets the unique identifier of the activity.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Gets the entity type involved in the activity.
    /// </summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the entity ID.
    /// </summary>
    public string EntityId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the operation performed.
    /// </summary>
    public string Operation { get; init; } = string.Empty;

    /// <summary>
    /// Gets the human-readable message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp of the activity.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the user ID who performed the action.
    /// </summary>
    public string? UserId { get; init; }
}

/// <summary>
/// Record containing dashboard statistics. Immutable by design.
/// </summary>
public record DashboardStatsDto
{
    /// <summary>
    /// Total number of persons in the system.
    /// </summary>
    public int TotalPersons { get; init; }

    /// <summary>
    /// Number of active employments.
    /// </summary>
    public int ActiveEmployments { get; init; }

    /// <summary>
    /// Total number of schools.
    /// </summary>
    public int TotalSchools { get; init; }

    /// <summary>
    /// Total number of positions.
    /// </summary>
    public int TotalPositions { get; init; }

    /// <summary>
    /// Total number of salary grades.
    /// </summary>
    public int TotalSalaryGrades { get; init; }

    /// <summary>
    /// Total number of items.
    /// </summary>
    public int TotalItems { get; init; }

    /// <summary>
    /// List of birthday celebrants this month.
    /// </summary>
    public IReadOnlyList<BirthdayCelebrantDto> BirthdayCelebrants { get; init; } = [];

    /// <summary>
    /// List of recent activities (last 10).
    /// </summary>
    public IReadOnlyList<RecentActivityDto> RecentActivities { get; init; } = [];
}
