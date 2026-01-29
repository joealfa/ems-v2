namespace EmployeeManagementSystem.Application.DTOs;

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
}
