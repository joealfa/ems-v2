using EmployeeManagementSystem.Application.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmployeeManagementSystem.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation for data access operations.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="Repository{T}"/> class.
/// </remarks>
public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.DisplayId == displayId, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<(List<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        query = orderBy != null
            ? descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy)
            : query.OrderBy(e => e.CreatedOn);

        List<T> items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        const int maxRetries = 3;
        int attempt = 0;

        while (true)
        {
            try
            {
                _ = await _dbSet.AddAsync(entity, cancellationToken);
                _ = await _context.SaveChangesAsync(cancellationToken);
                return entity;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex) && attempt < maxRetries)
            {
                attempt++;
                // Detach the entity and regenerate DisplayId
                _context.Entry(entity).State = EntityState.Detached;
                entity.RegenerateDisplayId();
            }
        }
    }

    /// <summary>
    /// Checks if the exception is caused by a unique constraint violation on DisplayId.
    /// </summary>
    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // Check for SQL Server unique constraint violation (error 2601 or 2627)
        // Also check the message for DisplayId to ensure it's the right constraint
        string message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("unique", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("2601") ||
               message.Contains("2627");
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _ = _dbSet.Update(entity);
        _ = await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Soft delete
        entity.IsDeleted = true;
        _ = _dbSet.Update(entity);
        _ = await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(long displayId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.DisplayId == displayId, cancellationToken);
    }

    /// <inheritdoc />
    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}
