using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents a salary grade in the system.
/// </summary>
public class SalaryGrade : BaseEntity
{
    /// <summary>
    /// Gets or sets the salary grade name/number.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SalaryGradeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the salary grade.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the step increment level.
    /// </summary>
    public int Step { get; set; } = 1;

    /// <summary>
    /// Gets or sets the monthly salary amount.
    /// </summary>
    public decimal MonthlySalary { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this salary grade is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the employment records associated with this salary grade.
    /// </summary>
    public virtual ICollection<Employment> Employments { get; set; } = [];
}
