using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spenses.Common.Extensions.Serialization;

/// <summary>
/// Allows a date with a time component to be deserialized as a <see cref="DateOnly"/>.
/// <remarks>
/// This is useful when working with JSON produced with flexible date/time types that include a time component by
/// default. For example, a TypeScript <c>Date</c> can deserialize <c>"2023-01-01"</c>, but will serialize to
/// <c>2023-01-01T00:00:00.000</c> by default.
/// </remarks>
/// </summary>
public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    /// <inheritdoc />
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var date = DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture);

        return DateOnly.FromDateTime(date);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O", CultureInfo.InvariantCulture);
        writer.WriteStringValue(isoDate);
    }
}
