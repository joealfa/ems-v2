using HotChocolate.Language;

namespace EmployeeManagementSystem.Gateway.Types.Configuration;

/// <summary>
/// Custom Long scalar type that accepts both string and integer input values.
/// This is needed because Apollo Client sends Long values as JSON numbers,
/// but HotChocolate's default Long scalar expects string input for precision.
/// </summary>
public class LongType : ScalarType<long>
{
    public LongType() : base("Long", BindingBehavior.Implicit)
    {
        Description = "The `Long` scalar type represents non-fractional signed whole 64-bit numeric values. " +
                      "Long can represent values between -(2^63) and 2^63 - 1.";
    }

    public override bool IsInstanceOfType(IValueNode valueSyntax)
    {
        return valueSyntax is null
            ? throw new ArgumentNullException(nameof(valueSyntax))
            : valueSyntax is IntValueNode or StringValueNode or NullValueNode;
    }

    public override object? ParseLiteral(IValueNode valueSyntax)
    {
        return valueSyntax is null
            ? throw new ArgumentNullException(nameof(valueSyntax))
            : valueSyntax switch
            {
                NullValueNode => null,
                IntValueNode intValue => intValue.ToInt64(),
                StringValueNode stringValue when long.TryParse(stringValue.Value, out long parsed) => parsed,
                _ => throw ThrowHelper.LongType_ParseLiteral_IsInvalid(this)
            };
    }

    public override IValueNode ParseValue(object? runtimeValue)
    {
        return runtimeValue switch
        {
            null => NullValueNode.Default,
            long l => new IntValueNode(l),
            int i => new IntValueNode(i),
            _ => throw ThrowHelper.LongType_ParseValue_IsInvalid(this)
        };
    }

    public override IValueNode ParseResult(object? resultValue)
    {
        return resultValue switch
        {
            null => NullValueNode.Default,
            long l => new IntValueNode(l),
            int i => new IntValueNode(i),
            string s when long.TryParse(s, out long parsed) => new IntValueNode(parsed),
            _ => throw ThrowHelper.LongType_ParseValue_IsInvalid(this)
        };
    }

    public override bool TrySerialize(object? runtimeValue, out object? resultValue)
    {
        switch (runtimeValue)
        {
            case null:
                resultValue = null;
                return true;
            case long l:
                resultValue = l;
                return true;
            case int i:
                resultValue = (long)i;
                return true;
            default:
                resultValue = null;
                return false;
        }
    }

    public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
    {
        switch (resultValue)
        {
            case null:
                runtimeValue = null;
                return true;
            case long l:
                runtimeValue = l;
                return true;
            case int i:
                runtimeValue = (long)i;
                return true;
            case string s when long.TryParse(s, out long parsed):
                runtimeValue = parsed;
                return true;
            default:
                runtimeValue = null;
                return false;
        }
    }
}

/// <summary>
/// Helper class for throwing consistent error messages
/// </summary>
internal static class ThrowHelper
{
    public static SerializationException LongType_ParseLiteral_IsInvalid(IType type)
    {
        return new SerializationException(
            "Unable to parse the literal value to a Long. Expected an integer or string value.",
            type);
    }

    public static SerializationException LongType_ParseValue_IsInvalid(IType type)
    {
        return new SerializationException(
            "Unable to parse the value to a Long. Expected an integer or string value.",
            type);
    }
}
