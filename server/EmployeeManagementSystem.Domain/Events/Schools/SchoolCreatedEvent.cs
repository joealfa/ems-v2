namespace EmployeeManagementSystem.Domain.Events.Schools;

public sealed class SchoolCreatedEvent : DomainEvent
{
    public SchoolCreatedEvent(Guid schoolId, string schoolName, bool isActive)
    {
        SchoolId = schoolId;
        SchoolName = schoolName;
        IsActive = isActive;
    }

    public Guid SchoolId { get; }
    public string SchoolName { get; }
    public bool IsActive { get; }

    public override string EventType => "com.ems.school.created";
}
