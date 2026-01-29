using EmployeeManagementSystem.Application.DTOs.Auth;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping User entities to DTOs.
/// </summary>
public static class UserMappingExtensions
{
    /// <summary>
    /// Maps a User entity to a UserDto.
    /// </summary>
    /// <param name="user">The user entity to map.</param>
    /// <returns>The mapped UserDto.</returns>
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role
        };
    }
}
