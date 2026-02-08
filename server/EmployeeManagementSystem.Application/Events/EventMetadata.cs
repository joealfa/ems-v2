using System.Text.Json.Serialization;

namespace EmployeeManagementSystem.Application.Events;

public sealed class EventMetadata
{
    [JsonPropertyName("ipAddress")]
    public string? IpAddress { get; init; }

    [JsonPropertyName("userAgent")]
    public string? UserAgent { get; init; }

    [JsonPropertyName("source")]
    public string? Source { get; init; }
}
