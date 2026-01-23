using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between Employment and School.
/// </summary>
public class EmploymentSchool : BaseEntity
{
    /// <summary>
    /// Gets or sets the employment ID.
    /// </summary>
    [Required]
    public Guid EmploymentId { get; set; }

    /// <summary>
    /// Gets or sets the associated employment.
    /// </summary>
    public virtual Employment Employment { get; set; } = null!;

    /// <summary>
    /// Gets or sets the school ID.
    /// </summary>
    [Required]
    public Guid SchoolId { get; set; }

    /// <summary>
    /// Gets or sets the associated school.
    /// </summary>
    public virtual School School { get; set; } = null!;

    /// <summary>
    /// Gets or sets the start date of the assignment.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the assignment.
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the current assignment.
    /// </summary>
    public bool IsCurrent { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this assignment is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
