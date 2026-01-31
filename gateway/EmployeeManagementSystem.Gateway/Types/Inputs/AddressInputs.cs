using EmployeeManagementSystem.ApiClient.Generated;

namespace EmployeeManagementSystem.Gateway.Types.Inputs;

[GraphQLDescription("Input for creating a new address")]
public class CreateAddressInput
{
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public AddressType? AddressType { get; set; }
    public string? Barangay { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool? IsCurrent { get; set; }
    public bool? IsPermanent { get; set; }
    public string? Province { get; set; }
    public string? ZipCode { get; set; }
}

[GraphQLDescription("Input for upserting an address (create or update)")]
public class UpsertAddressInput
{
    public long? DisplayId { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public AddressType? AddressType { get; set; }
    public string? Barangay { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool? IsCurrent { get; set; }
    public bool? IsPermanent { get; set; }
    public string? Province { get; set; }
    public string? ZipCode { get; set; }
}
