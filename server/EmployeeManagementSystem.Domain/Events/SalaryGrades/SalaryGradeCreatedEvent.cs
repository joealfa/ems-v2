namespace EmployeeManagementSystem.Domain.Events.SalaryGrades;

public sealed class SalaryGradeCreatedEvent : DomainEvent
{
    public SalaryGradeCreatedEvent(
        Guid salaryGradeId,
        string salaryGradeName,
        int step,
        decimal monthlySalary,
        bool isActive)
    {
        SalaryGradeId = salaryGradeId;
        SalaryGradeName = salaryGradeName;
        Step = step;
        MonthlySalary = monthlySalary;
        IsActive = isActive;
    }

    public Guid SalaryGradeId { get; }
    public string SalaryGradeName { get; }
    public int Step { get; }
    public decimal MonthlySalary { get; }
    public bool IsActive { get; }

    public override string EventType => "com.ems.salarygrade.created";
}
