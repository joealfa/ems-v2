using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing recent activities.
/// </summary>
public class RecentActivityRepository(ApplicationDbContext context) : IRecentActivityRepository
{
    private readonly ApplicationDbContext _context = context;

    /// <inheritdoc />
    public async Task AddAsync(RecentActivity activity, CancellationToken cancellationToken = default)
    {
        _ = await _context.RecentActivities.AddAsync(activity, cancellationToken);
        _ = await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<RecentActivity>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _context.RecentActivities
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
