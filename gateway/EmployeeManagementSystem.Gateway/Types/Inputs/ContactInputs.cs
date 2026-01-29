namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new contact")]
public class CreateContactInput
{
    public int? ContactType { get; set; }
    public string? Email { get; set; }
    public string? Fax { get; set; }
    public string? LandLine { get; set; }
    public string? Mobile { get; set; }
}

[GraphQLDescription("Input for upserting a contact (create or update)")]
public class UpsertContactInput
{
    public long? DisplayId { get; set; }
    public int? ContactType { get; set; }
    public string? Email { get; set; }
    public string? Fax { get; set; }
    public string? LandLine { get; set; }
    public string? Mobile { get; set; }
}
