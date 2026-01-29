namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new person")]
public class CreatePersonInput
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public int? CivilStatus { get; set; }
    public List<CreateAddressInput>? Addresses { get; set; }
    public List<CreateContactInput>? Contacts { get; set; }
}

[GraphQLDescription("Input for updating an existing person")]
public class UpdatePersonInput
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public int? CivilStatus { get; set; }
}
