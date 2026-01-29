namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new position")]
public class CreatePositionInput
{
    public string? TitleName { get; set; }
    public string? Description { get; set; }
}

[GraphQLDescription("Input for updating an existing position")]
public class UpdatePositionInput
{
    public string? TitleName { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
