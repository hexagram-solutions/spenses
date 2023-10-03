using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spenses.Application.Infrastructure;

public class LowerCaseNormalizingStringArrayConverter : JsonConverter<string[]>
{
    public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        reader.Read();

        var elements = new List<string>();

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            elements.Add(reader.GetString()!.ToLower());

            reader.Read();
        }

        return elements.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var s in value)
            writer.WriteStringValue(s.ToLower());

        writer.WriteEndArray();
    }
}
