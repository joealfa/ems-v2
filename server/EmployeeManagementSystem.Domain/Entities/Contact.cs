using EmployeeManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents contact information associated with a person or school.
/// </summary>
public class Contact : BaseEntity
{
    /// <summary>
    /// Gets or sets the mobile phone number.
    /// </summary>
    [MaxLength(20)]
    public string? Mobile { get; set; }

    /// <summary>
    /// Gets or sets the landline phone number.
    /// </summary>
    [MaxLength(20)]
    public string? LandLine { get; set; }

    /// <summary>
    /// Gets or sets the fax number.
    /// </summary>
    [MaxLength(20)]
    public string? Fax { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [MaxLength(256)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this contact is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of contact.
    /// </summary>
    [Required]
    public ContactType ContactType { get; set; }

    /// <summary>
    /// Gets or sets the associated person's ID (nullable for school contacts).
    /// </summary>
    public Guid? PersonId { get; set; }

    /// <summary>
    /// Gets or sets the associated person.
    /// </summary>
    public virtual Person? Person { get; set; }

    /// <summary>
    /// Gets or sets the associated school's ID (nullable for person contacts).
    /// </summary>
    public Guid? SchoolId { get; set; }

    /// <summary>
    /// Gets or sets the associated school.
    /// </summary>
    public virtual School? School { get; set; }
}
