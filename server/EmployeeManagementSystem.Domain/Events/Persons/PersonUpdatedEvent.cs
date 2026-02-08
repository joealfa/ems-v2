namespace EmployeeManagementSystem.Domain.Events.Persons;

public sealed class PersonUpdatedEvent : DomainEvent
{
    public PersonUpdatedEvent(
        int personId,
        Dictionary<string, object?> changes)
    {
        PersonId = personId;
        Changes = changes;
    }

    public int PersonId { get; }
    public Dictionary<string, object?> Changes { get; }

    public override string EventType => "com.ems.person.updated";
}
