using HotChocolate;
using HotChocolate.Types.Descriptors;

namespace EmployeeManagementSystem.Gateway.Types;

/// <summary>
/// Custom naming convention that preserves PascalCase for enum values
/// instead of converting them to UPPER_SNAKE_CASE (GraphQL default).
/// This allows enums like "SoloParent" to remain as "SoloParent" instead of "SOLO_PARENT".
/// </summary>
public class PascalCaseNamingConventions : DefaultNamingConventions
{
    public override string GetEnumValueName(object value)
    {
        // By default, value.ToString() in C# returns the PascalCase name of the enum member.
        return value.ToString() ?? string.Empty;
    }
}
