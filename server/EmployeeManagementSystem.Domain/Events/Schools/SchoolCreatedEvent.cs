namespace EmployeeManagementSystem.Domain.Events.Schools;

public sealed class SchoolCreatedEvent(Guid schoolId, string schoolName, bool isActive) : DomainEvent
{
    public Guid SchoolId { get; } = schoolId;
    public string SchoolName { get; } = schoolName;
    public bool IsActive { get; } = isActive;

    public override string EventType => "com.ems.school.created";
}
