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
}

/// <summary>
/// DTO for school response.
/// </summary>
public class SchoolResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the name of the school.
    /// </summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this school is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets or sets the addresses associated with the school.
    /// </summary>
    public List<AddressResponseDto> Addresses { get; init; } = [];

    /// <summary>
    /// Gets or sets the contacts associated with the school.
    /// </summary>
    public List<ContactResponseDto> Contacts { get; init; } = [];
}

/// <summary>
/// DTO for school list response (simplified).
/// </summary>
public class SchoolListDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the name of the school.
    /// </summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this school is active.
    /// </summary>
    public bool IsActive { get; init; }
}
