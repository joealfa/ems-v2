using EmployeeManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents an address associated with a person or school.
/// </summary>
public class Address : BaseEntity
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
    [Required]
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
    /// Gets or sets a value indicating whether this address is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of address.
    /// </summary>
    [Required]
    public AddressType AddressType { get; set; }

    /// <summary>
    /// Gets or sets the associated person's ID (nullable for school addresses).
    /// </summary>
    public Guid? PersonId { get; set; }

    /// <summary>
    /// Gets or sets the associated person.
    /// </summary>
    public virtual Person? Person { get; set; }

    /// <summary>
    /// Gets or sets the associated school's ID (nullable for person addresses).
    /// </summary>
    public Guid? SchoolId { get; set; }

    /// <summary>
    /// Gets or sets the associated school.
    /// </summary>
    public virtual School? School { get; set; }

    /// <summary>
    /// Gets the full address as a formatted string.
    /// </summary>
    public string FullAddress
    {
        get
        {
            List<string> parts = [Address1];
            if (!string.IsNullOrWhiteSpace(Address2))
            {
                parts.Add(Address2);
            }

            if (!string.IsNullOrWhiteSpace(Barangay))
            {
                parts.Add(Barangay);
            }

            parts.Add(City);
            parts.Add(Province);
            if (!string.IsNullOrWhiteSpace(ZipCode))
            {
                parts.Add(ZipCode);
            }

            parts.Add(Country);
            return string.Join(", ", parts);
        }
    }
}
