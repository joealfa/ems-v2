namespace EmployeeManagementSystem.Domain.Events.Persons;

public sealed class PersonDeletedEvent : DomainEvent
{
    public PersonDeletedEvent(int personId, string fullName)
    {
        PersonId = personId;
        FullName = fullName;
    }

    public int PersonId { get; }
    public string FullName { get; }

    public override string EventType => "com.ems.person.deleted";
}
