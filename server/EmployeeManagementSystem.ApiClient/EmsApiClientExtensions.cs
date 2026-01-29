using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace EmployeeManagementSystem.ApiClient.Generated;

/// <summary>
/// Partial class extension to configure JSON serialization settings for the EMS API client.
/// This is needed because the API returns enums as strings (using JsonStringEnumConverter),
/// but the NSwag-generated client has these fields typed as int.
/// </summary>
public partial class EmsApiClient
{
    /// <summary>
    /// Configures the JSON serializer settings to handle string-based enums.
    /// </summary>
    /// <param name="settings">The JSON serializer settings to configure.</param>
    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        // Add StringEnumConverter for actual enum types
        settings.Converters.Add(new StringEnumConverter());

        // Use custom contract resolver to apply StringToIntEnumConverter to specific properties
        settings.ContractResolver = new StringEnumContractResolver();
    }
}

/// <summary>
/// Custom contract resolver that applies StringToIntEnumConverter to known enum properties.
/// </summary>
public class StringEnumContractResolver : DefaultContractResolver
{
    // Known enum property names that should use string-to-int conversion
    private static readonly HashSet<string> EnumPropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "addressType",
        "contactType",
        "gender",
        "civilStatus",
        "employmentStatus"
    };

    protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Apply custom converter to int properties that are known enums
        if (property.PropertyType == typeof(int) && EnumPropertyNames.Contains(property.PropertyName ?? ""))
        {
            property.Converter = new StringToIntEnumConverter(property.PropertyName!);
        }

        return property;
    }
}

/// <summary>
/// Custom JSON converter that handles string enum values when the target type is int.
/// This is needed because the API returns enums as strings (e.g., "Business", "Personal")
/// but the NSwag-generated DTOs have these properties typed as int.
/// </summary>
public class StringToIntEnumConverter : JsonConverter
{
    private readonly string _propertyName;

    // Known enum mappings from the API - must match Domain/Enums definitions
    private static readonly Dictionary<string, Dictionary<string, int>> EnumMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        ["addressType"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Business"] = 1,
            ["Home"] = 2
        },
        ["contactType"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Work"] = 1,
            ["Personal"] = 2
        },
        ["gender"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Male"] = 1,
            ["Female"] = 2
        },
        ["civilStatus"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Single"] = 1,
            ["Married"] = 2,
            ["SoloParent"] = 3,
            ["Widow"] = 4,
            ["Separated"] = 5,
            ["Other"] = 99
        },
        ["employmentStatus"] = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Regular"] = 1,
            ["Permanent"] = 2
        }
    };

    public StringToIntEnumConverter(string propertyName)
    {
        _propertyName = propertyName;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(int);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return 0;
        }

        if (reader.TokenType == JsonToken.Integer)
        {
            return Convert.ToInt32(reader.Value);
        }

        if (reader.TokenType == JsonToken.String)
        {
            string? stringValue = reader.Value?.ToString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return 0;
            }

            // Try to parse as integer first
            if (int.TryParse(stringValue, out int intValue))
            {
                return intValue;
            }

            // Try to find matching enum mapping
            if (EnumMappings.TryGetValue(_propertyName, out Dictionary<string, int>? mapping) &&
                mapping.TryGetValue(stringValue, out int enumValue))
            {
                return enumValue;
            }

            // If no mapping found, throw an error
            throw new JsonSerializationException($"Cannot convert string '{stringValue}' to int for property '{_propertyName}'. No enum mapping found.");
        }

        throw new JsonSerializationException($"Unexpected token type {reader.TokenType} when parsing int for property '{_propertyName}'.");
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }

    public override bool CanWrite => true;
}
