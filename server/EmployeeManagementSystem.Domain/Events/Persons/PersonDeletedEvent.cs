namespace EmployeeManagementSystem.Domain.Events.Persons;

public sealed class PersonDeletedEvent(int personId, string fullName) : DomainEvent
{
    public int PersonId { get; } = personId;
    public string FullName { get; } = fullName;

    public override string EventType => "com.ems.person.deleted";
}
