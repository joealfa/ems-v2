using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Contact entities to DTOs.
/// </summary>
public static class ContactMappingExtensions
{
    /// <summary>
    /// Extension method for contact.
    /// </summary>
    /// <param name="contact"></param>
    extension(Contact contact)
    {
        /// <summary>
        /// Maps a Contact entity to a ContactResponseDto.
        /// </summary>
        /// <returns>The mapped ContactResponseDto.</returns>
        public ContactResponseDto ToResponseDto()
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
    }

    /// <summary>
    /// Extension method for contacts.
    /// </summary>
    /// <param name="contacts"></param>
    extension(IEnumerable<Contact> contacts)
    {
        /// <summary>
        /// Maps a collection of Contact entities to a list of ContactResponseDto.
        /// </summary>
        /// <returns>The list of mapped ContactResponseDto.</returns>
        public IReadOnlyList<ContactResponseDto> ToResponseDtoList()
        {
            return [.. contacts.Select(c => c.ToResponseDto())];
        }
    }
}
