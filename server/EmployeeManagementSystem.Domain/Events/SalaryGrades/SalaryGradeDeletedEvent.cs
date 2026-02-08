namespace EmployeeManagementSystem.Domain.Events.SalaryGrades;

public sealed class SalaryGradeDeletedEvent(Guid salaryGradeId, string salaryGradeName) : DomainEvent
{
    public Guid SalaryGradeId { get; } = salaryGradeId;
    public string SalaryGradeName { get; } = salaryGradeName;

    public override string EventType => "com.ems.salarygrade.deleted";
}
