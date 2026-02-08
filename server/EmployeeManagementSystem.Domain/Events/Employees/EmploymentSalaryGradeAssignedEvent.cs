namespace EmployeeManagementSystem.Domain.Events.Employees;

public sealed class EmploymentSalaryGradeAssignedEvent : DomainEvent
{
    public EmploymentSalaryGradeAssignedEvent(
        Guid employmentId,
        Guid salaryGradeId,
        string salaryGradeName,
        decimal monthlySalary,
        DateTime effectiveDate)
    {
        EmploymentId = employmentId;
        SalaryGradeId = salaryGradeId;
        SalaryGradeName = salaryGradeName;
        MonthlySalary = monthlySalary;
        EffectiveDate = effectiveDate;
    }

    public Guid EmploymentId { get; }
    public Guid SalaryGradeId { get; }
    public string SalaryGradeName { get; }
    public decimal MonthlySalary { get; }
    public DateTime EffectiveDate { get; }

    public override string EventType => "com.ems.employee.salarygrade.assigned";
}
