using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Application.DTOs.Item;

/// <summary>
/// DTO for creating a new item.
/// </summary>
public class CreateItemDto
{
    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the item.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating an existing item.
/// </summary>
public class UpdateItemDto
{
    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the item.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Record for item response data. Immutable by design.
/// </summary>
public record ItemResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    public string ItemName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description of the item.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets a value indicating whether this item is active.
    /// </summary>
    public bool IsActive { get; init; }
}
