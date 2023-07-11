using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spenses.Common.Extensions.Serialization;

/// <summary>
/// Converts a string to or from JSON, trimming trailing and leading whitespace from string values.
/// </summary>
public sealed class TrimmingJsonConverter : JsonConverter<string>
{
    /// <inheritdoc />
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()?.Trim();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Trim());
    }
}
