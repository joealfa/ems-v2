namespace EmployeeManagementSystem.Domain.Events.Schools;

public sealed class SchoolUpdatedEvent : DomainEvent
{
    public SchoolUpdatedEvent(Guid schoolId, Dictionary<string, object?> changes)
    {
        SchoolId = schoolId;
        Changes = changes;
    }

    public Guid SchoolId { get; }
    public Dictionary<string, object?> Changes { get; }

    public override string EventType => "com.ems.school.updated";
}
