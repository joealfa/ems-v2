namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentCreatedEvent(
    Guid employmentId,
    Guid personId,
    Guid positionId,
    Guid salaryGradeId,
    Guid itemId,
    string appointmentStatus,
    string employmentStatus) : DomainEvent
{
    public Guid EmploymentId { get; } = employmentId;
    public Guid PersonId { get; } = personId;
    public Guid PositionId { get; } = positionId;
    public Guid SalaryGradeId { get; } = salaryGradeId;
    public Guid ItemId { get; } = itemId;
    public string AppointmentStatus { get; } = appointmentStatus;
    public string EmploymentStatus { get; } = employmentStatus;

    public override string EventType => "com.ems.employee.created";
}
