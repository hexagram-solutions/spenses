using System.Text.Json.Serialization;

namespace Spenses.Application.Models.Homes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExpensePeriod
{
    Monthly,
    Weekly
}
