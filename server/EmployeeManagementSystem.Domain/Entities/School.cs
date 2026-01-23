using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a school in the system.
/// </summary>
public class School : BaseEntity
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
    /// Gets or sets the addresses associated with the school.
    /// </summary>
    public virtual ICollection<Address> Addresses { get; set; } = [];

    /// <summary>
    /// Gets or sets the contacts associated with the school.
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; set; } = [];

    /// <summary>
    /// Gets or sets the employment records associated with the school.
    /// </summary>
    public virtual ICollection<EmploymentSchool> EmploymentSchools { get; set; } = [];
}
