using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a job position in the system.
/// </summary>
public class Position : BaseEntity
{
    /// <summary>
    /// Gets or sets the title name of the position.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TitleName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the position.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this position is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the employment records associated with this position.
    /// </summary>
    public virtual ICollection<Employment> Employments { get; set; } = [];
}
