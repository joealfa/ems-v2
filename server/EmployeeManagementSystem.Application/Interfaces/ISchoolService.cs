using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.School;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for school operations.
/// </summary>
public interface ISchoolService
{
    /// <summary>
    /// Gets a school by display ID.
    /// </summary>
    Task<Result<SchoolResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of schools.
    /// </summary>
    Task<PagedResult<SchoolListDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new school.
    /// </summary>
    Task<Result<SchoolResponseDto>> CreateAsync(CreateSchoolDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing school.
    /// </summary>
    Task<Result<SchoolResponseDto>> UpdateAsync(long displayId, UpdateSchoolDto dto, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a school by display ID.
    /// </summary>
    Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default);
}
