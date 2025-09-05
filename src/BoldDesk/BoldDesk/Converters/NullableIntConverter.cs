using System.Text.Json;
using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class NullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetInt32();
            case JsonTokenType.String:
                var stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;
                if (int.TryParse(stringValue, out var result))
                    return result;
                return null;
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartObject:
                // Skip the entire object if the API returns an object instead of a simple value
                reader.Skip();
                return null;
            default:
                throw new JsonException($"Unexpected token type {reader.TokenType} when parsing nullable int");
        }
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}