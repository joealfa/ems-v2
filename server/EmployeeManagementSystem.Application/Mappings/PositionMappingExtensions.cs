using EmployeeManagementSystem.Application.DTOs.Position;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Position entities to DTOs.
/// </summary>
public static class PositionMappingExtensions
{
    /// <summary>
    /// Maps a Position entity to a PositionResponseDto.
    /// </summary>
    /// <param name="position">The position entity to map.</param>
    /// <returns>The mapped PositionResponseDto.</returns>
    public static PositionResponseDto ToResponseDto(this Position position)
    {
        return new PositionResponseDto
        {
            DisplayId = position.DisplayId,
            TitleName = position.TitleName,
            Description = position.Description,
            IsActive = position.IsActive,
            CreatedOn = position.CreatedOn,
            CreatedBy = position.CreatedBy,
            ModifiedOn = position.ModifiedOn,
            ModifiedBy = position.ModifiedBy
        };
    }
}
