using EmployeeManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
/// Record for employment response data. Immutable by design.
/// </summary>
public record EmploymentResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets the DepEd Employee ID.
    /// </summary>
    public string? DepEdId { get; init; }

    /// <summary>
    /// Gets the PSIPOP Item Number.
    /// </summary>
    public string? PSIPOPItemNumber { get; init; }

    /// <summary>
    /// Gets the Tax Identification Number.
    /// </summary>
    public string? TINId { get; init; }

    /// <summary>
    /// Gets the GSIS ID.
    /// </summary>
    public string? GSISId { get; init; }

    /// <summary>
    /// Gets the PhilHealth ID.
    /// </summary>
    public string? PhilHealthId { get; init; }

    /// <summary>
    /// Gets the date of original appointment.
    /// </summary>
    public DateOnly? DateOfOriginalAppointment { get; init; }

    /// <summary>
    /// Gets the appointment status.
    /// </summary>
    public AppointmentStatus AppointmentStatus { get; init; }

    /// <summary>
    /// Gets the employment status.
    /// </summary>
    public EmploymentStatus EmploymentStatus { get; init; }

    /// <summary>
    /// Gets the eligibility.
    /// </summary>
    public Eligibility Eligibility { get; init; }

    /// <summary>
    /// Gets a value indicating whether this employment is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the associated person.
    /// </summary>
    public EmploymentPersonDto Person { get; init; } = null!;

    /// <summary>
    /// Gets the associated position.
    /// </summary>
    public EmploymentPositionDto Position { get; init; } = null!;

    /// <summary>
    /// Gets the associated salary grade.
    /// </summary>
    public EmploymentSalaryGradeDto SalaryGrade { get; init; } = null!;

    /// <summary>
    /// Gets the associated item.
    /// </summary>
    public EmploymentItemDto Item { get; init; } = null!;

    /// <summary>
    /// Gets the school assignments.
    /// </summary>
    public IReadOnlyList<EmploymentSchoolResponseDto> Schools { get; init; } = [];
}

/// <summary>
/// Record for employment list response (simplified). Immutable by design.
/// </summary>
public record EmploymentListDto : BaseResponseDto
{
    /// <summary>
    /// Gets the DepEd Employee ID.
    /// </summary>
    public string? DepEdId { get; init; }

    /// <summary>
    /// Gets the employee's full name.
    /// </summary>
    public string EmployeeFullName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the position title.
    /// </summary>
    public string PositionTitle { get; init; } = string.Empty;

    /// <summary>
    /// Gets the employment status.
    /// </summary>
    public EmploymentStatus EmploymentStatus { get; init; }

    /// <summary>
    /// Gets a value indicating whether this employment is active.
    /// </summary>
    public bool IsActive { get; init; }
}

/// <summary>
/// Simplified person record for employment responses. Immutable by design.
/// </summary>
public record EmploymentPersonDto
{
    /// <summary>
    /// Gets the display ID of the person.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName { get; init; } = string.Empty;
}

/// <summary>
/// Simplified position record for employment responses. Immutable by design.
/// </summary>
public record EmploymentPositionDto
{
    /// <summary>
    /// Gets the display ID of the position.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets the title name of the position.
    /// </summary>
    public string TitleName { get; init; } = string.Empty;
}

/// <summary>
/// Simplified salary grade record for employment responses. Immutable by design.
/// </summary>
public record EmploymentSalaryGradeDto
{
    /// <summary>
    /// Gets the display ID of the salary grade.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets the salary grade name.
    /// </summary>
    public string SalaryGradeName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the step.
    /// </summary>
    public int Step { get; init; }

    /// <summary>
    /// Gets the monthly salary.
    /// </summary>
    public decimal MonthlySalary { get; init; }
}

/// <summary>
/// Simplified item record for employment responses. Immutable by design.
/// </summary>
public record EmploymentItemDto
{
    /// <summary>
    /// Gets the display ID of the item.
    /// </summary>
    public long DisplayId { get; init; }

    /// <summary>
    /// Gets the item name.
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
/// Record for employment-school assignment response. Immutable by design.
/// </summary>
public record EmploymentSchoolResponseDto : BaseResponseDto
{
    /// <summary>
    /// Gets the display ID of the school.
    /// </summary>
    public long SchoolDisplayId { get; init; }

    /// <summary>
    /// Gets the school name.
    /// </summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the start date of the assignment.
    /// </summary>
    public DateOnly? StartDate { get; init; }

    /// <summary>
    /// Gets the end date of the assignment.
    /// </summary>
    public DateOnly? EndDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether this is the current assignment.
    /// </summary>
    public bool IsCurrent { get; init; }

    /// <summary>
    /// Gets a value indicating whether this assignment is active.
    /// </summary>
    public bool IsActive { get; init; }
}

/// <summary>
/// Query parameters for employment pagination with column filtering support.
/// </summary>
public class EmploymentPaginationQuery : PaginationQuery
{
    /// <summary>
    /// Gets or sets the employment status filter.
    /// </summary>
    public EmploymentStatus? EmploymentStatus { get; set; }

    /// <summary>
    /// Gets or sets the is active filter.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the display ID filter (contains search).
    /// </summary>
    public string? DisplayIdFilter { get; set; }

    /// <summary>
    /// Gets or sets the employee name filter (contains search).
    /// </summary>
    public string? EmployeeNameFilter { get; set; }

    /// <summary>
    /// Gets or sets the DepEd ID filter (contains search).
    /// </summary>
    public string? DepEdIdFilter { get; set; }

    /// <summary>
    /// Gets or sets the position filter (contains search).
    /// </summary>
    public string? PositionFilter { get; set; }
}
