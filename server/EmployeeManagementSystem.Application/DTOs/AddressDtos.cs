using System.ComponentModel.DataAnnotations;
using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Application.DTOs;

/// <summary>
/// DTO for creating a new address.
/// </summary>
public class CreateAddressDto
{
    /// <summary>
    /// Gets or sets the primary address line.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secondary address line.
    /// </summary>
    [MaxLength(200)]
    public string? Address2 { get; set; }

    /// <summary>
    /// Gets or sets the barangay.
    /// </summary>
    [MaxLength(100)]
    public string? Barangay { get; set; }

    /// <summary>
    /// Gets or sets the city or municipality.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the province or state.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [MaxLength(100)]
    public string Country { get; set; } = "Philippines";

    /// <summary>
    /// Gets or sets the zip or postal code.
    /// </summary>
    [MaxLength(20)]
    public string? ZipCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the current address.
    /// </summary>
    public bool IsCurrent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the permanent address.
    /// </summary>
    public bool IsPermanent { get; set; }

    /// <summary>
    /// Gets or sets the type of address.
    /// </summary>
    [Required]
    public AddressType AddressType { get; set; }
}

/// <summary>
/// DTO for updating an existing address.
/// </summary>
public class UpdateAddressDto : CreateAddressDto
{
    /// <summary>
    /// Gets or sets the display ID of the address to update.
    /// </summary>
    [Required]
    public long DisplayId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this address is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for address response.
/// </summary>
public class AddressResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the primary address line.
    /// </summary>
    public string Address1 { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the secondary address line.
    /// </summary>
    public string? Address2 { get; init; }

    /// <summary>
    /// Gets or sets the barangay.
    /// </summary>
    public string? Barangay { get; init; }

    /// <summary>
    /// Gets or sets the city or municipality.
    /// </summary>
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the province or state.
    /// </summary>
    public string Province { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the zip or postal code.
    /// </summary>
    public string? ZipCode { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the current address.
    /// </summary>
    public bool IsCurrent { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the permanent address.
    /// </summary>
    public bool IsPermanent { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this address is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets or sets the type of address.
    /// </summary>
    public AddressType AddressType { get; init; }

    /// <summary>
    /// Gets the full address as a formatted string.
    /// </summary>
    public string FullAddress { get; init; } = string.Empty;
}
