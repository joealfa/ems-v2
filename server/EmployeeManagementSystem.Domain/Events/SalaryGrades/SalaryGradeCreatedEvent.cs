namespace EmployeeManagementSystem.Domain.Events.SalaryGrades;

public sealed class SalaryGradeCreatedEvent(
    Guid salaryGradeId,
    string salaryGradeName,
    int step,
    decimal monthlySalary,
    bool isActive) : DomainEvent
{
    public Guid SalaryGradeId { get; } = salaryGradeId;
    public string SalaryGradeName { get; } = salaryGradeName;
    public int Step { get; } = step;
    public decimal MonthlySalary { get; } = monthlySalary;
    public bool IsActive { get; } = isActive;

    public override string EventType => "com.ems.salarygrade.created";
}
