namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentSalaryGradeAssignedEvent(
    Guid employmentId,
    Guid salaryGradeId,
    string salaryGradeName,
    decimal monthlySalary,
    DateTime effectiveDate) : DomainEvent
{
    public Guid EmploymentId { get; } = employmentId;
    public Guid SalaryGradeId { get; } = salaryGradeId;
    public string SalaryGradeName { get; } = salaryGradeName;
    public decimal MonthlySalary { get; } = monthlySalary;
    public DateTime EffectiveDate { get; } = effectiveDate;

    public override string EventType => "com.ems.employee.salarygrade.assigned";
}
