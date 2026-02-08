namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentPositionAssignedEvent : DomainEvent
{
    public EmploymentPositionAssignedEvent(
        Guid employmentId,
        Guid positionId,
        string positionTitle,
        DateTime effectiveDate)
    {
        EmploymentId = employmentId;
        PositionId = positionId;
        PositionTitle = positionTitle;
        EffectiveDate = effectiveDate;
    }

    public Guid EmploymentId { get; }
    public Guid PositionId { get; }
    public string PositionTitle { get; }
    public DateTime EffectiveDate { get; }

    public override string EventType => "com.ems.employee.position.assigned";
}
