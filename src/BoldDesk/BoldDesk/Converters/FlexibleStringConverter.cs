using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Converts a JSON value that might be a string, number, bool, or object into a readable string.
/// Used for BoldDesk responses that sometimes return nested objects instead of strings.
/// </summary>
public class FlexibleStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.True:
            case JsonTokenType.False:
                return reader.GetBoolean().ToString();
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var longValue))
                    return longValue.ToString(CultureInfo.InvariantCulture);
                if (reader.TryGetDouble(out var doubleValue))
                    return doubleValue.ToString(CultureInfo.InvariantCulture);
                var decimalValue = reader.GetDecimal();
                return decimalValue.ToString(CultureInfo.InvariantCulture);
            case JsonTokenType.StartObject:
                using (var doc = JsonDocument.ParseValue(ref reader))
                {
                    var root = doc.RootElement;
                    if (TryGetStringProperty(root, "brandName", out var brandName))
                        return brandName;
                    if (TryGetStringProperty(root, "name", out var name))
                        return name;
                    if (TryGetStringProperty(root, "displayName", out var displayName))
                        return displayName;
                    return root.GetRawText();
                }
            case JsonTokenType.StartArray:
                using (var doc = JsonDocument.ParseValue(ref reader))
                {
                    return doc.RootElement.GetRawText();
                }
            default:
                throw new JsonException($"Unexpected token type {reader.TokenType} when parsing string value.");
        }
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value);
        }
    }

    private static bool TryGetStringProperty(JsonElement element, string propertyName, out string? value)
    {
        if (element.ValueKind == JsonValueKind.Object &&
            element.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.String)
        {
            value = property.GetString();
            return true;
        }

        value = null;
        return false;
    }
}
