using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Item entities to DTOs.
/// </summary>
public static class ItemMappingExtensions
{
    /// <summary>
    /// Maps an Item entity to an ItemResponseDto.
    /// </summary>
    /// <param name="item">The item entity to map.</param>
    /// <returns>The mapped ItemResponseDto.</returns>
    public static ItemResponseDto ToResponseDto(this Item item)
    {
        return new ItemResponseDto
        {
            DisplayId = item.DisplayId,
            ItemName = item.ItemName,
            Description = item.Description,
            IsActive = item.IsActive,
            CreatedOn = item.CreatedOn,
            CreatedBy = item.CreatedBy,
            ModifiedOn = item.ModifiedOn,
            ModifiedBy = item.ModifiedBy
        };
    }
}
