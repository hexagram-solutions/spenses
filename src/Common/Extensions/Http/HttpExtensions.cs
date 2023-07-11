using System.Reflection;

namespace Spenses.Common.Extensions.Http;

/// <summary>
/// Extension methods for working with HTTP messages.
/// </summary>
public static class HttpExtensions
{
    /// <summary>
    /// Serialize an object into an HTTP query string.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="obj">The <typeparamref name="T" /> to serialize.</param>
    /// <returns>A query string containing the object properties.</returns>
    public static string ToQueryString<T>(this T obj)
        where T : class
    {
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var valuesByPropertyName = properties.ToDictionary(k => k.Name, v => v.GetValue(obj));

        var queryParams = new Dictionary<string, string>();

        foreach (var property in valuesByPropertyName)
        {
            var stringValue = property.Value?.ToString();

            if (string.IsNullOrEmpty(stringValue))
                continue;

            queryParams.Add(property.Key, stringValue);
        }

        return queryParams.Any()
            ? $"?{string.Join("&", queryParams.Select(qp => $"{qp.Key}={qp.Value}").ToArray())}"
            : string.Empty;
    }
}
