using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Contact entities to DTOs.
/// </summary>
public static class ContactMappingExtensions
{
    /// <summary>
    /// Maps a Contact entity to a ContactResponseDto.
    /// </summary>
    /// <param name="contact">The contact entity to map.</param>
    /// <returns>The mapped ContactResponseDto.</returns>
    public static ContactResponseDto ToResponseDto(this Contact contact)
    {
        return new ContactResponseDto
        {
            DisplayId = contact.DisplayId,
            Mobile = contact.Mobile,
            LandLine = contact.LandLine,
            Fax = contact.Fax,
            Email = contact.Email,
            IsActive = contact.IsActive,
            ContactType = contact.ContactType,
            CreatedOn = contact.CreatedOn,
            CreatedBy = contact.CreatedBy,
            ModifiedOn = contact.ModifiedOn,
            ModifiedBy = contact.ModifiedBy
        };
    }

    /// <summary>
    /// Maps a collection of Contact entities to ContactResponseDto list.
    /// </summary>
    /// <param name="contacts">The contact entities to map.</param>
    /// <returns>The mapped list of ContactResponseDto.</returns>
    public static IReadOnlyList<ContactResponseDto> ToResponseDtoList(this IEnumerable<Contact> contacts)
    {
        return contacts.Select(c => c.ToResponseDto()).ToList();
    }
}
