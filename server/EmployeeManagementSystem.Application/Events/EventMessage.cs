using System.Text.Json.Serialization;

namespace EmployeeManagementSystem.Application.Events;

/// <summary>
/// Event message payload (inside CloudEvent.data)
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
    public string Timestamp { get; init; } = DateTime.UtcNow.ToString("O");

    [JsonPropertyName("userId")]
    public string? UserId { get; init; }

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }

    [JsonPropertyName("payload")]
    public object? Payload { get; init; }

    [JsonPropertyName("changes")]
    public List<FieldChange>? Changes { get; init; }

    [JsonPropertyName("metadata")]
    public EventMetadata? Metadata { get; init; }
}

public sealed class FieldChange
{
    [JsonPropertyName("field")]
    public string Field { get; init; } = string.Empty;

    [JsonPropertyName("oldValue")]
    public object? OldValue { get; init; }

    [JsonPropertyName("newValue")]
    public object? NewValue { get; init; }
}
