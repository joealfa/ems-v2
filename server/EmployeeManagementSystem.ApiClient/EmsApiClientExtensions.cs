using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EmployeeManagementSystem.ApiClient.Generated;

/// <summary>
/// Partial class extension to configure JSON serialization settings for the EMS API client.
/// Configures the client to serialize/deserialize enums as strings to match the API behavior.
/// </summary>
public partial class EmsApiClient
{
    /// <summary>
    /// Configures the JSON serializer settings to handle string-based enums.
    /// </summary>
    /// <param name="settings">The JSON serializer settings to configure.</param>
    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        // Add StringEnumConverter to serialize/deserialize enums as strings
        // This matches the API server configuration which uses System.Text.Json.JsonStringEnumConverter
        settings.Converters.Add(new StringEnumConverter());
    }
}
