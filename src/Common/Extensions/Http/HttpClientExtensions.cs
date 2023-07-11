using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Spenses.Common.Extensions.Serialization;

namespace Spenses.Common.Extensions.Http;

/// <summary>
/// Extension methods for <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a POST request to the specified Uri containing the <paramref name="value"/> serialized as JSON in the
    /// request body and deserializes the response from JSON to a new instance of <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of the response to deserialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client"/> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(this HttpClient client, string? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(requestUri, value, options, cancellationToken)
            .ConfigureAwait(false);

        return await response
            .EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<TResponse>(options, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request to the specified Uri containing the <paramref name="value"/> serialized as JSON in the
    /// request body and deserializes the response from JSON to a new instance of <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of the response to deserialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client"/> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(this HttpClient client, string? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(requestUri, value, options, cancellationToken).ConfigureAwait(false);

        return await response
            .EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<TResponse>(options, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request to the specified Uri containing the <paramref name="value"/> serialized as JSON in the
    /// request body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client"/> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="value"/> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static async Task<HttpResponseMessage?> DeleteAsJsonAsync<TRequest>(this HttpClient client,
        string? requestUri, TRequest value, JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        options ??= new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
        {
            Content = new StringContent(value.ToJson(options), Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

        return response.EnsureSuccessStatusCode();
    }
}
