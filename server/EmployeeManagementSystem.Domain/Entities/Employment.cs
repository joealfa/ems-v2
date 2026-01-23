using System.ComponentModel.DataAnnotations;
using EmployeeManagementSystem.Domain.Enums;

namespace EmployeeManagementSystem.Domain.Entities;

/// <summary>
/// Represents an employment record in the system.
/// </summary>
public class Employment : BaseEntity
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
    /// Gets or sets a value indicating whether this employment is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the associated person's ID.
    /// </summary>
    [Required]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Gets or sets the associated person.
    /// </summary>
    public virtual Person Person { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated position's ID.
    /// </summary>
    [Required]
    public Guid PositionId { get; set; }

    /// <summary>
    /// Gets or sets the associated position.
    /// </summary>
    public virtual Position Position { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated salary grade's ID.
    /// </summary>
    [Required]
    public Guid SalaryGradeId { get; set; }

    /// <summary>
    /// Gets or sets the associated salary grade.
    /// </summary>
    public virtual SalaryGrade SalaryGrade { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated item's ID.
    /// </summary>
    [Required]
    public Guid ItemId { get; set; }

    /// <summary>
    /// Gets or sets the associated item.
    /// </summary>
    public virtual Item Item { get; set; } = null!;

    /// <summary>
    /// Gets or sets the schools associated with this employment.
    /// </summary>
    public virtual ICollection<EmploymentSchool> EmploymentSchools { get; set; } = [];
}
