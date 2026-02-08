namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentCreatedEvent : DomainEvent
{
    public EmploymentCreatedEvent(
        Guid employmentId,
        Guid personId,
        Guid positionId,
        Guid salaryGradeId,
        Guid itemId,
        string appointmentStatus,
        string employmentStatus)
    {
        EmploymentId = employmentId;
        PersonId = personId;
        PositionId = positionId;
        SalaryGradeId = salaryGradeId;
        ItemId = itemId;
        AppointmentStatus = appointmentStatus;
        EmploymentStatus = employmentStatus;
    }

    public Guid EmploymentId { get; }
    public Guid PersonId { get; }
    public Guid PositionId { get; }
    public Guid SalaryGradeId { get; }
    public Guid ItemId { get; }
    public string AppointmentStatus { get; }
    public string EmploymentStatus { get; }

    public override string EventType => "com.ems.employee.created";
}
