using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a plantilla item in the system.
/// </summary>
public class Item : BaseEntity
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

    /// <summary>
    /// Gets or sets the employment records associated with this item.
    /// </summary>
    public virtual ICollection<Employment> Employments { get; set; } = [];
}
