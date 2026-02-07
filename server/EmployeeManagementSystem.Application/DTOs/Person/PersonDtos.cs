using EmployeeManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Application.DTOs.Person;

/// <summary>
/// DTO for creating a new person.
/// </summary>
public class CreatePersonDto
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the middle name of the person.
    /// </summary>
    [MaxLength(100)]
    public string? MiddleName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth of the person.
    /// </summary>
    [Required]
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the gender of the person.
    /// </summary>
    [Required]
    public Gender Gender { get; set; }

    /// <summary>
    /// Gets or sets the civil status of the person.
    /// </summary>
    [Required]
    public CivilStatus CivilStatus { get; set; }

    /// <summary>
    /// Gets or sets the addresses associated with the person.
    /// </summary>
    public List<CreateAddressDto>? Addresses { get; set; }

    /// <summary>
    /// Gets or sets the contacts associated with the person.
    /// </summary>
    public List<CreateContactDto>? Contacts { get; set; }
}

/// <summary>
/// DTO for updating an existing person.
/// </summary>
public class UpdatePersonDto
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the middle name of the person.
    /// </summary>
    [MaxLength(100)]
    public string? MiddleName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth of the person.
    /// </summary>
    [Required]
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the gender of the person.
    /// </summary>
    [Required]
    public Gender Gender { get; set; }

    /// <summary>
    /// Gets or sets the civil status of the person.
    /// </summary>
    [Required]
    public CivilStatus CivilStatus { get; set; }

    /// <summary>
    /// Gets or sets the addresses to upsert for the person.
    /// Addresses not in the list will be soft-deleted.
    /// </summary>
    public List<UpsertAddressDto>? Addresses { get; set; }

    /// <summary>
    /// Gets or sets the contacts to upsert for the person.
    /// Contacts not in the list will be soft-deleted.
    /// </summary>
    public List<UpsertContactDto>? Contacts { get; set; }
}

/// <summary>
/// Record for person response data. Immutable by design.
/// </summary>
public record PersonResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets the first name of the person.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the last name of the person.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the middle name of the person.
    /// </summary>
    public string? MiddleName { get; init; }

    /// <summary>
    /// Gets the date of birth of the person.
    /// </summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets the gender of the person.
    /// </summary>
    public Gender Gender { get; init; }

    /// <summary>
    /// Gets the civil status of the person.
    /// </summary>
    public CivilStatus CivilStatus { get; init; }

    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the addresses associated with the person.
    /// </summary>
    public IReadOnlyList<AddressResponseDto> Addresses { get; init; } = [];

    /// <summary>
    /// Gets the contacts associated with the person.
    /// </summary>
    public IReadOnlyList<ContactResponseDto> Contacts { get; init; } = [];

    /// <summary>
    /// Gets the URL of the person's profile image.
    /// </summary>
    public string? ProfileImageUrl { get; init; }

    /// <summary>
    /// Gets a value indicating whether the person has a profile image.
    /// </summary>
    public bool HasProfileImage { get; init; }
}

/// <summary>
/// Record for person list response (simplified). Immutable by design.
/// </summary>
public record PersonListDto : BaseResponseDto
{
    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the date of birth of the person.
    /// </summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets the gender of the person.
    /// </summary>
    public Gender Gender { get; init; }

    /// <summary>
    /// Gets the civil status of the person.
    /// </summary>
    public CivilStatus CivilStatus { get; init; }

    /// <summary>
    /// Gets the URL of the person's profile image.
    /// </summary>
    public string? ProfileImageUrl { get; init; }

    /// <summary>
    /// Gets a value indicating whether the person has a profile image.
    /// </summary>
    public bool HasProfileImage { get; init; }
}

/// <summary>
/// Query parameters for person pagination with column filtering support.
/// </summary>
public class PersonPaginationQuery : PaginationQuery
{
    /// <summary>
    /// Gets or sets the gender filter.
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Gets or sets the civil status filter.
    /// </summary>
    public CivilStatus? CivilStatus { get; set; }

    /// <summary>
    /// Gets or sets the display ID filter (contains search).
    /// </summary>
    public string? DisplayIdFilter { get; set; }

    /// <summary>
    /// Gets or sets the full name filter (contains search).
    /// </summary>
    public string? FullNameFilter { get; set; }
}
