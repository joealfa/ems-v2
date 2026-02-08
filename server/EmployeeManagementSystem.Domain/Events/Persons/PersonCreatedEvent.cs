namespace EmployeeManagementSystem.Domain.Events.Persons;

public sealed class PersonCreatedEvent : DomainEvent
{
    public PersonCreatedEvent(
        int personId,
        string firstName,
        string lastName,
        string? middleName,
        DateOnly dateOfBirth,
        string gender,
        string civilStatus)
    {
        PersonId = personId;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        CivilStatus = civilStatus;
    }

    public int PersonId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }
    public DateOnly DateOfBirth { get; }
    public string Gender { get; }
    public string CivilStatus { get; }

    public override string EventType => "com.ems.person.created";
}
