namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new salary grade")]
public class CreateSalaryGradeInput
{
    public string? SalaryGradeName { get; set; }
    public string? Description { get; set; }
    public int? Step { get; set; }
    public decimal? MonthlySalary { get; set; }
}

[GraphQLDescription("Input for updating an existing salary grade")]
public class UpdateSalaryGradeInput
{
    public string? SalaryGradeName { get; set; }
    public string? Description { get; set; }
    public int? Step { get; set; }
    public decimal? MonthlySalary { get; set; }
    public bool? IsActive { get; set; }
}
