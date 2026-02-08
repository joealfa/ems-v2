namespace EmployeeManagementSystem.Domain.Events.SalaryGrades;

public sealed class SalaryGradeDeletedEvent : DomainEvent
{
    public SalaryGradeDeletedEvent(Guid salaryGradeId, string salaryGradeName)
    {
        SalaryGradeId = salaryGradeId;
        SalaryGradeName = salaryGradeName;
    }

    public Guid SalaryGradeId { get; }
    public string SalaryGradeName { get; }

    public override string EventType => "com.ems.salarygrade.deleted";
}
