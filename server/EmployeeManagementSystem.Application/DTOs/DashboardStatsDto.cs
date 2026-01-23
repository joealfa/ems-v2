namespace EmployeeManagementSystem.Application.DTOs;

/// <summary>
/// DTO containing dashboard statistics.
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Total number of persons in the system.
    /// </summary>
    public int TotalPersons { get; set; }

    /// <summary>
    /// Number of active employments.
    /// </summary>
    public int ActiveEmployments { get; set; }

    /// <summary>
    /// Total number of schools.
    /// </summary>
    public int TotalSchools { get; set; }

    /// <summary>
    /// Total number of positions.
    /// </summary>
    public int TotalPositions { get; set; }

    /// <summary>
    /// Total number of salary grades.
    /// </summary>
    public int TotalSalaryGrades { get; set; }

    /// <summary>
    /// Total number of items.
    /// </summary>
    public int TotalItems { get; set; }
}
