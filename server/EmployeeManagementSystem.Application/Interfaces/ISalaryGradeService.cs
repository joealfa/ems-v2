using EmployeeManagementSystem.Application.Common;
using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.SalaryGrade;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for salary grade operations.
/// </summary>
public interface ISalaryGradeService
{
    /// <summary>
    /// Gets a salary grade by display ID.
    /// </summary>
    Task<Result<SalaryGradeResponseDto>> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of salary grades.
    /// </summary>
    Task<PagedResult<SalaryGradeResponseDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new salary grade.
    /// </summary>
    Task<Result<SalaryGradeResponseDto>> CreateAsync(CreateSalaryGradeDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing salary grade.
    /// </summary>
    Task<Result<SalaryGradeResponseDto>> UpdateAsync(long displayId, UpdateSalaryGradeDto dto, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a salary grade by display ID.
    /// </summary>
    Task<Result> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default);
}
