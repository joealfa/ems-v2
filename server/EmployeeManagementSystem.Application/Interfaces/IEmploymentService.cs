using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Employment;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for employment operations.
/// </summary>
public interface IEmploymentService
{
    /// <summary>
    /// Gets an employment record by display ID.
    /// </summary>
    Task<Result<EmploymentResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of employment records.
    /// </summary>
    Task<PagedResult<EmploymentListDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new employment record.
    /// </summary>
    Task<Result<EmploymentResponseDto>> CreateAsync(CreateEmploymentDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing employment record.
    /// </summary>
    Task<Result<EmploymentResponseDto>> UpdateAsync(long displayId, UpdateEmploymentDto dto, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an employment record by display ID.
    /// </summary>
    Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a school assignment to an employment record.
    /// </summary>
    Task<Result<EmploymentSchoolResponseDto>> AddSchoolAssignmentAsync(long employmentDisplayId, CreateEmploymentSchoolDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a school assignment from an employment record.
    /// </summary>
    Task<Result> RemoveSchoolAssignmentAsync(long employmentSchoolDisplayId, string deletedBy, CancellationToken cancellationToken = default);
}
