using System.ComponentModel.DataAnnotations;
using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Application.DTOs.Employment;

/// <summary>
/// DTO for creating a new employment record.
/// </summary>
public class CreateEmploymentDto
{
    /// <summary>
    /// Gets or sets the DepEd Employee ID.
    /// </summary>
    [MaxLength(50)]
    public string? DepEdId { get; set; }

    /// <summary>
    /// Gets or sets the PSIPOP Item Number.
    /// </summary>
    [MaxLength(50)]
    public string? PSIPOPItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the Tax Identification Number.
    /// </summary>
    [MaxLength(20)]
    public string? TINId { get; set; }

    /// <summary>
    /// Gets or sets the GSIS ID.
    /// </summary>
    [MaxLength(20)]
    public string? GSISId { get; set; }

    /// <summary>
    /// Gets or sets the PhilHealth ID.
    /// </summary>
    [MaxLength(20)]
    public string? PhilHealthId { get; set; }

    /// <summary>
    /// Gets or sets the date of original appointment.
    /// </summary>
    public DateOnly? DateOfOriginalAppointment { get; set; }

    /// <summary>
    /// Gets or sets the appointment status.
    /// </summary>
    [Required]
    public AppointmentStatus AppointmentStatus { get; set; }

    /// <summary>
    /// Gets or sets the employment status.
    /// </summary>
    [Required]
    public EmploymentStatus EmploymentStatus { get; set; }

    /// <summary>
    /// Gets or sets the eligibility.
    /// </summary>
    [Required]
    public Eligibility Eligibility { get; set; }

    /// <summary>
    /// Gets or sets the display ID of the associated person.
    /// </summary>
    [Required]
    public long PersonDisplayId { get; set; }

    /// <summary>
    /// Gets or sets the display ID of the associated position.
    /// </summary>
    [Required]
    public long PositionDisplayId { get; set; }

    /// <summary>
    /// Gets or sets the display ID of the associated salary grade.
    /// </summary>
    [Required]
    public long SalaryGradeDisplayId { get; set; }

    /// <summary>
    /// Gets or sets the display ID of the associated item.
    /// </summary>
    [Required]
    public long ItemDisplayId { get; set; }

    /// <summary>
    /// Gets or sets the school assignments.
    /// </summary>
    public List<CreateEmploymentSchoolDto>? Schools { get; set; }
}

/// <summary>
/// DTO for updating an existing employment record.
/// </summary>
public class UpdateEmploymentDto
{
    /// <summary>
    /// Gets or sets the DepEd Employee ID.
    /// </summary>
    [MaxLength(50)]
    public string? DepEdId { get; set; }

    /// <summary>
    /// Gets or sets the PSIPOP Item Number.
    /// </summary>
    [MaxLength(50)]
    public string? PSIPOPItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the Tax Identification Number.
    /// </summary>
    [MaxLength(20)]
    public string? TINId { get; set; }

    /// <summary>
    /// Gets or sets the GSIS ID.
    /// </summary>
    [MaxLength(20)]
    public string? GSISId { get; set; }

    /// <summary>
    /// Gets or sets the PhilHealth ID.
    /// </summary>
    [MaxLength(20)]
    public string? PhilHealthId { get; set; }

    /// <summary>
    /// Gets or sets the date of original appointment.
    /// </summary>
    public DateOnly? DateOfOriginalAppointment { get; set; }

    /// <summary>
    /// Gets or sets the appointment status.
    /// </summary>
    [Required]
    public AppointmentStatus AppointmentStatus { get; set; }

    /// <summary>
    /// Gets or sets the employment status.
    /// </summary>
    [Required]
    public EmploymentStatus EmploymentStatus { get; set; }

    /// <summary>
    /// Gets or sets the eligibility.
    /// </summary>
    [Required]
    public Eligibility Eligibility { get; set; }

    /// <summary>
    /// Gets or sets the display ID of the associated position.
    /// </summary>
    [Required]
    public long PositionDisplayId { get; set; }

    /// <summary>
    /// Gets or sets the display ID of the associated salary grade.
    /// </summary>
    [Required]
    public long SalaryGradeDisplayId { get; set; }

    /// <summary>
    /// Gets or sets the display ID of the associated item.
    /// </summary>
    [Required]
    public long ItemDisplayId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this employment is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for employment response.
/// </summary>
public class EmploymentResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the DepEd Employee ID.
    /// </summary>
    public string? DepEdId { get; init; }

    /// <summary>
    /// Gets or sets the PSIPOP Item Number.
    /// </summary>
    public string? PSIPOPItemNumber { get; init; }

    /// <summary>
    /// Gets or sets the Tax Identification Number.
    /// </summary>
    public string? TINId { get; init; }

    /// <summary>
    /// Gets or sets the GSIS ID.
    /// </summary>
    public string? GSISId { get; init; }

    /// <summary>
    /// Gets or sets the PhilHealth ID.
    /// </summary>
    public string? PhilHealthId { get; init; }

    /// <summary>
    /// Gets or sets the date of original appointment.
    /// </summary>
    public DateOnly? DateOfOriginalAppointment { get; init; }

    /// <summary>
    /// Gets or sets the appointment status.
    /// </summary>
    public AppointmentStatus AppointmentStatus { get; init; }

    /// <summary>
    /// Gets or sets the employment status.
    /// </summary>
    public EmploymentStatus EmploymentStatus { get; init; }

    /// <summary>
    /// Gets or sets the eligibility.
    /// </summary>
    public Eligibility Eligibility { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this employment is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets or sets the associated person.
    /// </summary>
    public EmploymentPersonDto Person { get; init; } = null!;

    /// <summary>
    /// Gets or sets the associated position.
    /// </summary>
    public EmploymentPositionDto Position { get; init; } = null!;

    /// <summary>
    /// Gets or sets the associated salary grade.
    /// </summary>
    public EmploymentSalaryGradeDto SalaryGrade { get; init; } = null!;

    /// <summary>
    /// Gets or sets the associated item.
    /// </summary>
    public EmploymentItemDto Item { get; init; } = null!;

    /// <summary>
    /// Gets or sets the school assignments.
    /// </summary>
    public List<EmploymentSchoolResponseDto> Schools { get; init; } = [];
}

/// <summary>
/// DTO for employment list response (simplified).
/// </summary>
public class EmploymentListDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the DepEd Employee ID.
    /// </summary>
    public string? DepEdId { get; init; }

    /// <summary>
    /// Gets or sets the employee's full name.
    /// </summary>
    public string EmployeeFullName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the position title.
    /// </summary>
    public string PositionTitle { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the employment status.
    /// </summary>
    public EmploymentStatus EmploymentStatus { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this employment is active.
    /// </summary>
    public bool IsActive { get; init; }
}

/// <summary>
/// Simplified person DTO for employment responses.
/// </summary>
public class EmploymentPersonDto
{
    /// <summary>
    /// Gets or sets the display ID of the person.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets or sets the full name of the person.
    /// </summary>
    public string FullName { get; init; } = string.Empty;
}

/// <summary>
/// Simplified position DTO for employment responses.
/// </summary>
public class EmploymentPositionDto
{
    /// <summary>
    /// Gets or sets the display ID of the position.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets or sets the title name of the position.
    /// </summary>
    public string TitleName { get; init; } = string.Empty;
}

/// <summary>
/// Simplified salary grade DTO for employment responses.
/// </summary>
public class EmploymentSalaryGradeDto
{
    /// <summary>
    /// Gets or sets the display ID of the salary grade.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets or sets the salary grade name.
    /// </summary>
    public string SalaryGradeName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the step.
    /// </summary>
    public int Step { get; init; }

    /// <summary>
    /// Gets or sets the monthly salary.
    /// </summary>
    public decimal MonthlySalary { get; init; }
}

/// <summary>
/// Simplified item DTO for employment responses.
/// </summary>
public class EmploymentItemDto
{
    /// <summary>
    /// Gets or sets the display ID of the item.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets or sets the item name.
    /// </summary>
    public string ItemName { get; init; } = string.Empty;
}

/// <summary>
/// DTO for creating an employment-school assignment.
/// </summary>
public class CreateEmploymentSchoolDto
{
    /// <summary>
    /// Gets or sets the display ID of the school.
    /// </summary>
    [Required]
    public long SchoolDisplayId { get; set; }

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
}

/// <summary>
/// DTO for employment-school assignment response.
/// </summary>
public class EmploymentSchoolResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets or sets the display ID of the school.
    /// </summary>
    public long SchoolDisplayId { get; init; }

    /// <summary>
    /// Gets or sets the school name.
    /// </summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the assignment.
    /// </summary>
    public DateOnly? StartDate { get; init; }

    /// <summary>
    /// Gets or sets the end date of the assignment.
    /// </summary>
    public DateOnly? EndDate { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the current assignment.
    /// </summary>
    public bool IsCurrent { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this assignment is active.
    /// </summary>
    public bool IsActive { get; init; }
}
