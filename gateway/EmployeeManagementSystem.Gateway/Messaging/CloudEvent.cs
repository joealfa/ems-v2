using System.Text.Json.Serialization;

namespace EmployeeManagementSystem.Gateway.Messaging;

/// <summary>
/// CloudEvents v1.0 compliant event wrapper for deserialization.
/// </summary>
public sealed class CloudEvent<T>
{
    [JsonPropertyName("specversion")]
    public string SpecVersion { get; init; } = "1.0";

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("time")]
    public string Time { get; init; } = string.Empty;

    [JsonPropertyName("datacontenttype")]
    public string DataContentType { get; init; } = "application/json";

    [JsonPropertyName("data")]
    public T Data { get; init; } = default!;
}

/// <summary>
/// Generic event message payload structure.
/// </summary>
public sealed class EventMessage
{
    [JsonPropertyName("entityType")]
    public string EntityType { get; init; } = string.Empty;

    [JsonPropertyName("entityId")]
    public string EntityId { get; init; } = string.Empty;

    [JsonPropertyName("operation")]
    public string Operation { get; init; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }

    [JsonPropertyName("payload")]
    public Dictionary<string, object?>? Payload { get; init; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string?>? Metadata { get; init; }
}
