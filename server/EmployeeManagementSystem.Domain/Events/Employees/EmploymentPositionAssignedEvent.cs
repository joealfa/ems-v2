namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentPositionAssignedEvent(
    Guid employmentId,
    Guid positionId,
    string positionTitle,
    DateTime effectiveDate) : DomainEvent
{
    public Guid EmploymentId { get; } = employmentId;
    public Guid PositionId { get; } = positionId;
    public string PositionTitle { get; } = positionTitle;
    public DateTime EffectiveDate { get; } = effectiveDate;

    public override string EventType => "com.ems.employee.position.assigned";
}
