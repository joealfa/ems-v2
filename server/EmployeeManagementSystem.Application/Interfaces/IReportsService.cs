using EmployeeManagementSystem.Application.DTOs;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for reports and statistics.
/// </summary>
public interface IReportsService
{
    /// <summary>
    /// Gets dashboard statistics including counts of persons, employments, schools, and positions.
    /// </summary>
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}
