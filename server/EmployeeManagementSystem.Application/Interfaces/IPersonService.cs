using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Application.DTOs.Person;

namespace EmployeeManagementSystem.Application.Interfaces;

/// <summary>
/// Service interface for person operations.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Gets a person by display ID.
    /// </summary>
    Task<PersonResponseDto?> GetByDisplayIdAsync(long displayId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of persons.
    /// </summary>
    Task<PagedResult<PersonListDto>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new person.
    /// </summary>
    Task<PersonResponseDto> CreateAsync(CreatePersonDto dto, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    Task<PersonResponseDto?> UpdateAsync(long displayId, UpdatePersonDto dto, string modifiedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a person by display ID.
    /// </summary>
    Task<bool> DeleteAsync(long displayId, string deletedBy, CancellationToken cancellationToken = default);
}
