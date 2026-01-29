namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new employment")]
public class CreateEmploymentInput
{
    public long? PersonDisplayId { get; set; }
    public long? PositionDisplayId { get; set; }
    public long? SalaryGradeDisplayId { get; set; }
    public long? ItemDisplayId { get; set; }
    public int? EmploymentStatus { get; set; }
    public int? AppointmentStatus { get; set; }
    public int? Eligibility { get; set; }
    public DateTime? DateOfOriginalAppointment { get; set; }
    public string? PsipopItemNumber { get; set; }
    public string? DepEdId { get; set; }
    public string? GsisId { get; set; }
    public string? PhilHealthId { get; set; }
    public string? TinId { get; set; }
    public List<CreateEmploymentSchoolInput>? Schools { get; set; }
}

[GraphQLDescription("Input for updating an existing employment")]
public class UpdateEmploymentInput
{
    public long? PositionDisplayId { get; set; }
    public long? SalaryGradeDisplayId { get; set; }
    public long? ItemDisplayId { get; set; }
    public int? EmploymentStatus { get; set; }
    public int? AppointmentStatus { get; set; }
    public int? Eligibility { get; set; }
    public DateTime? DateOfOriginalAppointment { get; set; }
    public string? PsipopItemNumber { get; set; }
    public string? DepEdId { get; set; }
    public string? GsisId { get; set; }
    public string? PhilHealthId { get; set; }
    public string? TinId { get; set; }
    public bool? IsActive { get; set; }
}

[GraphQLDescription("Input for associating a school with an employment")]
public class CreateEmploymentSchoolInput
{
    public long? SchoolDisplayId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsCurrent { get; set; }
}
