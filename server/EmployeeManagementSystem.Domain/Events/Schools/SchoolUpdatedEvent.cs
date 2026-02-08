namespace EmployeeManagementSystem.Domain.Events.Schools;

public sealed class SchoolUpdatedEvent(Guid schoolId, Dictionary<string, object?> changes) : DomainEvent
{
    public Guid SchoolId { get; } = schoolId;
    public Dictionary<string, object?> Changes { get; } = changes;

    public override string EventType => "com.ems.school.updated";
}
