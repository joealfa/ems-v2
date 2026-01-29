namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new item")]
public class CreateItemInput
{
    public string? ItemName { get; set; }
    public string? Description { get; set; }
}

[GraphQLDescription("Input for updating an existing item")]
public class UpdateItemInput
{
    public string? ItemName { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
