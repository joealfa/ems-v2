namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentDeletedEvent : DomainEvent
{
    public EmploymentDeletedEvent(Guid employmentId, Guid personId)
    {
        EmploymentId = employmentId;
        PersonId = personId;
    }

    public Guid EmploymentId { get; }
    public Guid PersonId { get; }

    public override string EventType => "com.ems.employee.deleted";
}
