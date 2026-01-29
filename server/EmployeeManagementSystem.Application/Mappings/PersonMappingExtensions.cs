using EmployeeManagementSystem.Application.DTOs.Person;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Person entities to DTOs.
/// </summary>
public static class PersonMappingExtensions
{
    /// <summary>
    /// Maps a Person entity to a PersonResponseDto.
    /// </summary>
    /// <param name="person">The person entity to map.</param>
    /// <returns>The mapped PersonResponseDto.</returns>
    public static PersonResponseDto ToResponseDto(this Person person)
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
    /// <param name="person">The person entity to map.</param>
    /// <returns>The mapped PersonListDto.</returns>
    public static PersonListDto ToListDto(this Person person)
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
}
