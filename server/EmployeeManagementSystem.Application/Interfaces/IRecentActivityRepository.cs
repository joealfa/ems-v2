using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Repository interface for managing recent activities.
/// </summary>
public interface IRecentActivityRepository
{
    /// <summary>
    /// Adds a new recent activity entry to the database.
    /// </summary>
    /// <param name="activity">The activity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(RecentActivity activity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest N activities ordered by timestamp descending.
    /// </summary>
    /// <param name="count">Number of activities to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of recent activities.</returns>
    Task<List<RecentActivity>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
}
