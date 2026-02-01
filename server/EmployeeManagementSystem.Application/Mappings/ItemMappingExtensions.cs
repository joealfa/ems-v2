using EmployeeManagementSystem.Application.DTOs.Employment;
using EmployeeManagementSystem.Application.DTOs.Item;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Item entities to DTOs.
/// </summary>
public static class ItemMappingExtensions
{
    /// <summary>
    /// Extension method for item.
    /// </summary>
    /// <param name="item">The item entity to map.</param>
    extension(Item item)
    {
        /// <summary>
        /// Maps an Item entity to an ItemResponseDto.
        /// </summary>
        /// <returns>The mapped ItemResponseDto.</returns>
        public ItemResponseDto ToResponseDto()
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

        /// <summary>
        /// Maps an Item entity to an EmploymentItemDto (simplified view for employment context).
        /// </summary>
        /// <returns>The mapped EmploymentItemDto.</returns>
        public EmploymentItemDto ToEmploymentItemDto()
        {
            return new EmploymentItemDto
            {
                DisplayId = item.DisplayId,
                ItemName = item.ItemName
            };
        }
    }
}
