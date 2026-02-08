namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentUpdatedEvent : DomainEvent
{
    public EmploymentUpdatedEvent(Guid employmentId, Dictionary<string, object?> changes)
    {
        EmploymentId = employmentId;
        Changes = changes;
    }

    public Guid EmploymentId { get; }
    public Dictionary<string, object?> Changes { get; }

    public override string EventType => "com.ems.employee.updated";
}
