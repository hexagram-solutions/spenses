using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spenses.Shared.Common.Serialization;

public class LowerCaseNormalizingStringListConverter : JsonConverter<List<string>>
{
    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        return [.. elements];
    }

    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var s in value)
            writer.WriteStringValue(s.ToLower());

        writer.WriteEndArray();
    }
}
