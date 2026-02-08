namespace EmployeeManagementSystem.Domain.Events.Persons;

public sealed class PersonCreatedEvent(
    int personId,
    string firstName,
    string lastName,
    string? middleName,
    DateOnly dateOfBirth,
    string gender,
    string civilStatus) : DomainEvent
{
    public int PersonId { get; } = personId;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string? MiddleName { get; } = middleName;
    public DateOnly DateOfBirth { get; } = dateOfBirth;
    public string Gender { get; } = gender;
    public string CivilStatus { get; } = civilStatus;

    public override string EventType => "com.ems.person.created";
}
