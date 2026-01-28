using EmployeeManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Application.DTOs;

/// <summary>
/// DTO for creating a new contact.
/// </summary>
public class CreateContactDto
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
    /// Gets or sets the type of contact.
    /// </summary>
    [Required]
    public ContactType ContactType { get; set; }
}

/// <summary>
/// DTO for updating an existing contact.
/// </summary>
public class UpdateContactDto : CreateContactDto
{
    /// <summary>
    /// Gets or sets the display ID of the contact to update.
    /// </summary>
    [Required]
    public long DisplayId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this contact is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for upserting a contact (create new or update existing).
/// When DisplayId is null, a new contact is created. When DisplayId has a value, the existing contact is updated.
/// </summary>
public class UpsertContactDto
{
    /// <summary>
    /// Gets or sets the display ID of the contact. Null for new contacts.
    /// </summary>
    public long? DisplayId { get; set; }

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
    /// Gets or sets the type of contact.
    /// </summary>
    [Required]
    public ContactType ContactType { get; set; }
}

/// <summary>
/// DTO for contact response.
/// </summary>
public class ContactResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the mobile phone number.
    /// </summary>
    public string? Mobile { get; init; }

    /// <summary>
    /// Gets or sets the landline phone number.
    /// </summary>
    public string? LandLine { get; init; }

    /// <summary>
    /// Gets or sets the fax number.
    /// </summary>
    public string? Fax { get; init; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this contact is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets or sets the type of contact.
    /// </summary>
    public ContactType ContactType { get; init; }
}
