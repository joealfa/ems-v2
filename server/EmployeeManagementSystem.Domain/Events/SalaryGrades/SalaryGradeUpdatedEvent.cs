namespace EmployeeManagementSystem.Domain.Events.SalaryGrades;

public sealed class SalaryGradeUpdatedEvent : DomainEvent
{
    public SalaryGradeUpdatedEvent(Guid salaryGradeId, Dictionary<string, object?> changes)
    {
        SalaryGradeId = salaryGradeId;
        Changes = changes;
    }

    public Guid SalaryGradeId { get; }
    public Dictionary<string, object?> Changes { get; }

    public override string EventType => "com.ems.salarygrade.updated";
}
