using EmployeeManagementSystem.Application.DTOs.School;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping School entities to DTOs.
/// </summary>
public static class SchoolMappingExtensions
{
    /// <summary>
    /// Extension method for school.
    /// </summary>
    /// <param name="school">The school entity to map.</param>
    extension(School school)
    {
        /// <summary>
        /// Maps a School entity to a SchoolResponseDto.
        /// </summary>
        /// <returns>The mapped SchoolResponseDto.</returns>
        public SchoolResponseDto ToResponseDto()
        {
            return new SchoolResponseDto
            {
                DisplayId = school.DisplayId,
                SchoolName = school.SchoolName,
                IsActive = school.IsActive,
                CreatedOn = school.CreatedOn,
                CreatedBy = school.CreatedBy,
                ModifiedOn = school.ModifiedOn,
                ModifiedBy = school.ModifiedBy,
                Addresses = school.Addresses.ToResponseDtoList(),
                Contacts = school.Contacts.ToResponseDtoList()
            };
        }

        /// <summary>
        /// Maps a School entity to a SchoolListDto.
        /// </summary>
        /// <returns>The mapped SchoolListDto.</returns>
        public SchoolListDto ToListDto()
        {
            return new SchoolListDto
            {
                DisplayId = school.DisplayId,
                SchoolName = school.SchoolName,
                IsActive = school.IsActive,
                CreatedOn = school.CreatedOn,
                CreatedBy = school.CreatedBy,
                ModifiedOn = school.ModifiedOn,
                ModifiedBy = school.ModifiedBy
            };
        }
    }
}
