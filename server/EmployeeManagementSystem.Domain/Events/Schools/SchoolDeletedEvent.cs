namespace EmployeeManagementSystem.Domain.Events.Schools;

public sealed class SchoolDeletedEvent : DomainEvent
{
    public SchoolDeletedEvent(Guid schoolId, string schoolName)
    {
        SchoolId = schoolId;
        SchoolName = schoolName;
    }

    public Guid SchoolId { get; }
    public string SchoolName { get; }

    public override string EventType => "com.ems.school.deleted";
}
