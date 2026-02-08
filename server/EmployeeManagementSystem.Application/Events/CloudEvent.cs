using System.Text.Json.Serialization;

namespace EmployeeManagementSystem.Application.Events;

/// <summary>
/// CloudEvents v1.0 compliant event wrapper
/// </summary>
public sealed class CloudEvent<T>
{
    /// <summary>
    /// CloudEvents specification version (always "1.0")
    /// </summary>
    [JsonPropertyName("specversion")]
    public string SpecVersion { get; init; } = "1.0";

    /// <summary>
    /// Event type (e.g., "com.ems.person.created")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Source system identifier
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; init; } = "ems-backend-api";

    /// <summary>
    /// Unique event identifier
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Timestamp when event occurred (ISO 8601)
    /// </summary>
    [JsonPropertyName("time")]
    public string Time { get; init; } = DateTime.UtcNow.ToString("O");

    /// <summary>
    /// Content type of data (always "application/json")
    /// </summary>
    [JsonPropertyName("datacontenttype")]
    public string DataContentType { get; init; } = "application/json";

    /// <summary>
    /// Event payload
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; init; } = default!;
}
