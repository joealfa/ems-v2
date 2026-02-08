namespace EmployeeManagementSystem.Domain.Events.SalaryGrades;

public sealed class SalaryGradeUpdatedEvent(Guid salaryGradeId, Dictionary<string, object?> changes) : DomainEvent
{
    public Guid SalaryGradeId { get; } = salaryGradeId;
    public Dictionary<string, object?> Changes { get; } = changes;

    public override string EventType => "com.ems.salarygrade.updated";
}
