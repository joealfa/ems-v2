using EmployeeManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a person in the system.
/// </summary>
public class Person : BaseEntity
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
    public virtual ICollection<Address> Addresses { get; set; } = [];

    /// <summary>
    /// Gets or sets the contacts associated with the person.
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; set; } = [];

    /// <summary>
    /// Gets or sets the employment records associated with the person.
    /// </summary>
    public virtual ICollection<Employment> Employments { get; set; } = [];

    /// <summary>
    /// Gets or sets the documents associated with the person.
    /// </summary>
    public virtual ICollection<Document> Documents { get; set; } = [];

    /// <summary>
    /// Gets or sets the URL of the person's profile image in Azure Blob Storage.
    /// </summary>
    [MaxLength(2048)]
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person has a profile image.
    /// </summary>
    public bool HasProfileImage { get; set; }

    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName => string.IsNullOrWhiteSpace(MiddleName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";
}
