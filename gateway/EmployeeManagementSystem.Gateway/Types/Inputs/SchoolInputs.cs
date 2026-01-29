namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new school")]
public class CreateSchoolInput
{
    public string? SchoolName { get; set; }
    public List<CreateAddressInput>? Addresses { get; set; }
    public List<CreateContactInput>? Contacts { get; set; }
}

[GraphQLDescription("Input for updating an existing school")]
public class UpdateSchoolInput
{
    public string? SchoolName { get; set; }
    public List<UpsertAddressInput>? Addresses { get; set; }
    public List<UpsertContactInput>? Contacts { get; set; }
    public bool? IsActive { get; set; }
}
