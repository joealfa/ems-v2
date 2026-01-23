using System.ComponentModel.DataAnnotations;
using EmployeeManagementSystem.Domain.Enums;

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
}

/// <summary>
/// DTO for person response.
/// </summary>
public class PersonResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the middle name of the person.
    /// </summary>
    public string? MiddleName { get; init; }

    /// <summary>
    /// Gets or sets the date of birth of the person.
    /// </summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets or sets the gender of the person.
    /// </summary>
    public Gender Gender { get; init; }

    /// <summary>
    /// Gets or sets the civil status of the person.
    /// </summary>
    public CivilStatus CivilStatus { get; init; }

    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the addresses associated with the person.
    /// </summary>
    public List<AddressResponseDto> Addresses { get; init; } = [];

    /// <summary>
    /// Gets or sets the contacts associated with the person.
    /// </summary>
    public List<ContactResponseDto> Contacts { get; init; } = [];

    /// <summary>
    /// Gets or sets the URL of the person's profile image.
    /// </summary>
    public string? ProfileImageUrl { get; init; }
}

/// <summary>
/// DTO for person list response (simplified).
/// </summary>
public class PersonListDto : BaseResponseDto
{
    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of birth of the person.
    /// </summary>
    public DateOnly DateOfBirth { get; init; }

    /// <summary>
    /// Gets or sets the gender of the person.
    /// </summary>
    public Gender Gender { get; init; }

    /// <summary>
    /// Gets or sets the civil status of the person.
    /// </summary>
    public CivilStatus CivilStatus { get; init; }

    /// <summary>
    /// Gets or sets the URL of the person's profile image.
    /// </summary>
    public string? ProfileImageUrl { get; init; }
}
