using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.DTOs.Person;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Person entities to DTOs.
/// </summary>
public static class PersonMappingExtensions
{
    /// <summary>
    /// Extension method for person.
    /// </summary>
    /// <param name="person">The person entity to map.</param>
    extension(Person person)
    {
        /// <summary>
        /// Maps a Person entity to a PersonResponseDto.
        /// </summary>
        /// <returns>The mapped PersonResponseDto.</returns>
        public PersonResponseDto ToResponseDto()
        {
            return new PersonResponseDto
            {
                DisplayId = person.DisplayId,
                FirstName = person.FirstName,
                LastName = person.LastName,
                MiddleName = person.MiddleName,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CivilStatus = person.CivilStatus,
                FullName = person.FullName,
                ProfileImageUrl = person.ProfileImageUrl,
                CreatedOn = person.CreatedOn,
                CreatedBy = person.CreatedBy,
                ModifiedOn = person.ModifiedOn,
                ModifiedBy = person.ModifiedBy,
                Addresses = person.Addresses.ToResponseDtoList(),
                Contacts = person.Contacts.ToResponseDtoList()
            };
        }

        /// <summary>
        /// Maps a Person entity to a PersonListDto.
        /// </summary>
        /// <returns>The mapped PersonListDto.</returns>
        public PersonListDto ToListDto()
        {
            return new PersonListDto
            {
                DisplayId = person.DisplayId,
                FullName = person.FullName,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CivilStatus = person.CivilStatus,
                ProfileImageUrl = person.ProfileImageUrl,
                CreatedOn = person.CreatedOn,
                CreatedBy = person.CreatedBy,
                ModifiedOn = person.ModifiedOn,
                ModifiedBy = person.ModifiedBy
            };
        }

        /// <summary>
        /// Maps a Person entity to an EmploymentPersonDto (simplified view for employment context).
        /// </summary>
        /// <returns>The mapped EmploymentPersonDto.</returns>
        public EmploymentPersonDto ToEmploymentPersonDto()
        {
            return new EmploymentPersonDto
            {
                DisplayId = person.DisplayId,
                FullName = person.FullName
            };
        }
    }
}
