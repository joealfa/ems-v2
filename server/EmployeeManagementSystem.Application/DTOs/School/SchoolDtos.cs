using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Application.DTOs.School;

/// <summary>
/// DTO for creating a new school.
/// </summary>
public class CreateSchoolDto
{
    /// <summary>
    /// Gets or sets the name of the school.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string SchoolName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the addresses associated with the school.
    /// </summary>
    public List<CreateAddressDto>? Addresses { get; set; }

    /// <summary>
    /// Gets or sets the contacts associated with the school.
    /// </summary>
    public List<CreateContactDto>? Contacts { get; set; }
}

/// <summary>
/// DTO for updating an existing school.
/// </summary>
public class UpdateSchoolDto
{
    /// <summary>
    /// Gets or sets the name of the school.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string SchoolName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this school is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the addresses to upsert. Existing addresses not in this list will be soft-deleted.
    /// </summary>
    public List<UpsertAddressDto>? Addresses { get; set; }

    /// <summary>
    /// Gets or sets the contacts to upsert. Existing contacts not in this list will be soft-deleted.
    /// </summary>
    public List<UpsertContactDto>? Contacts { get; set; }
}

/// <summary>
/// Record for school response data. Immutable by design.
/// </summary>
public record SchoolResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets the name of the school.
    /// </summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this school is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the addresses associated with the school.
    /// </summary>
    public IReadOnlyList<AddressResponseDto> Addresses { get; init; } = [];

    /// <summary>
    /// Gets the contacts associated with the school.
    /// </summary>
    public IReadOnlyList<ContactResponseDto> Contacts { get; init; } = [];
}

/// <summary>
/// Record for school list response (simplified). Immutable by design.
/// </summary>
public record SchoolListDto : BaseResponseDto
{
    /// <summary>
    /// Gets the name of the school.
    /// </summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this school is active.
    /// </summary>
    public bool IsActive { get; init; }
}
