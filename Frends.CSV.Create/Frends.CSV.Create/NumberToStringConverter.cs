using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frends.CSV.Create;

class NumberToStringConverter : JsonConverter<JsonElement>
{
    public override JsonElement Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }

    public override void Write(
        Utf8JsonWriter writer,
        JsonElement value,
        JsonSerializerOptions options
    )
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Number:
                writer.WriteStringValue(value.GetRawText());
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in value.EnumerateArray())
                {
                    Write(writer, item, options);
                }
                writer.WriteEndArray();
                break;
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var prop in value.EnumerateObject())
                {
                    writer.WritePropertyName(prop.Name);
                    Write(writer, prop.Value, options);
                }
                writer.WriteEndObject();
                break;
            default:
                value.WriteTo(writer);
                break;
        }
    }
}
