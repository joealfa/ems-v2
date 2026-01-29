using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Application.DTOs.SalaryGrade;

/// <summary>
/// DTO for creating a new salary grade.
/// </summary>
public class CreateSalaryGradeDto
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
    [Range(1, 8)]
    public int Step { get; set; } = 1;

    /// <summary>
    /// Gets or sets the monthly salary amount.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal MonthlySalary { get; set; }
}

/// <summary>
/// DTO for updating an existing salary grade.
/// </summary>
public class UpdateSalaryGradeDto
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
    [Range(1, 8)]
    public int Step { get; set; } = 1;

    /// <summary>
    /// Gets or sets the monthly salary amount.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal MonthlySalary { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this salary grade is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Record for salary grade response data. Immutable by design.
/// </summary>
public record SalaryGradeResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets the salary grade name/number.
    /// </summary>
    public string SalaryGradeName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description of the salary grade.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the step increment level.
    /// </summary>
    public int Step { get; init; }

    /// <summary>
    /// Gets the monthly salary amount.
    /// </summary>
    public decimal MonthlySalary { get; init; }

    /// <summary>
    /// Gets a value indicating whether this salary grade is active.
    /// </summary>
    public bool IsActive { get; init; }
}
