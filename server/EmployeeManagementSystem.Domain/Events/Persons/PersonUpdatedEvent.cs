namespace EmployeeManagementSystem.Domain.Events.Persons;

public sealed class PersonUpdatedEvent(
    int personId,
    Dictionary<string, object?> changes) : DomainEvent
{
    public int PersonId { get; } = personId;
    public Dictionary<string, object?> Changes { get; } = changes;

    public override string EventType => "com.ems.person.updated";
}
