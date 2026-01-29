using EmployeeManagementSystem.Application.DTOs.School;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping School entities to DTOs.
/// </summary>
public static class SchoolMappingExtensions
{
    /// <summary>
    /// Maps a School entity to a SchoolResponseDto.
    /// </summary>
    /// <param name="school">The school entity to map.</param>
    /// <returns>The mapped SchoolResponseDto.</returns>
    public static SchoolResponseDto ToResponseDto(this School school)
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
    /// <param name="school">The school entity to map.</param>
    /// <returns>The mapped SchoolListDto.</returns>
    public static SchoolListDto ToListDto(this School school)
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
