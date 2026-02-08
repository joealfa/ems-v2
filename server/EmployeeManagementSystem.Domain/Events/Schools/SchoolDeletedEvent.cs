namespace EmployeeManagementSystem.Domain.Events.Schools;

public sealed class SchoolDeletedEvent(Guid schoolId, string schoolName) : DomainEvent
{
    public Guid SchoolId { get; } = schoolId;
    public string SchoolName { get; } = schoolName;

    public override string EventType => "com.ems.school.deleted";
}
