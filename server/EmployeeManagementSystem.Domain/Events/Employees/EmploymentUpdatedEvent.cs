namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentUpdatedEvent(Guid employmentId, Dictionary<string, object?> changes) : DomainEvent
{
    public Guid EmploymentId { get; } = employmentId;
    public Dictionary<string, object?> Changes { get; } = changes;

    public override string EventType => "com.ems.employee.updated";
}
