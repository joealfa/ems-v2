namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentDeletedEvent(Guid employmentId, Guid personId) : DomainEvent
{
    public Guid EmploymentId { get; } = employmentId;
    public Guid PersonId { get; } = personId;

    public override string EventType => "com.ems.employee.deleted";
}
