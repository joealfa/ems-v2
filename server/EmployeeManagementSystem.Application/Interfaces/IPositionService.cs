using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Position;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for position operations.
/// </summary>
public interface IPositionService
{
    /// <summary>
    /// Gets a position by display ID.
    /// </summary>
    Task<PositionResponseDto?> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of positions.
    /// </summary>
    Task<PagedResult<PositionResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new position.
    /// </summary>
    Task<PositionResponseDto> CreateAsync(CreatePositionDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing position.
    /// </summary>
    Task<PositionResponseDto?> UpdateAsync(long displayId, UpdatePositionDto dto, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a position by display ID.
    /// </summary>
    Task<bool> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default);
}
