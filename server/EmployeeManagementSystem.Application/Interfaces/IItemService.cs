using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Item;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for item operations.
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Gets an item by display ID.
    /// </summary>
    Task<Result<ItemResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of items.
    /// </summary>
    Task<PagedResult<ItemResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new item.
    /// </summary>
    Task<Result<ItemResponseDto>> CreateAsync(CreateItemDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    Task<Result<ItemResponseDto>> UpdateAsync(long displayId, UpdateItemDto dto, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an item by display ID.
    /// </summary>
    Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default);
}
