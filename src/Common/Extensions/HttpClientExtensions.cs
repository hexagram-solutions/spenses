using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Spenses.Common.Extensions.Serialization;

namespace Spenses.Common.Extensions;

public static class HttpClientExtensions
{
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(this HttpClient client, string? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var postResponse = await client.PostAsJsonAsync(requestUri, value, options, cancellationToken);

        postResponse.EnsureSuccessStatusCode();

        return await postResponse.Content.ReadFromJsonAsync<TResponse>(options, cancellationToken);
    }

    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(this HttpClient client, string? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var postResponse = await client.PutAsJsonAsync(requestUri, value, options, cancellationToken);

        postResponse.EnsureSuccessStatusCode();

        return await postResponse.Content.ReadFromJsonAsync<TResponse>(options, cancellationToken);
    }

    public static async Task<HttpResponseMessage?> DeleteAsync<T>(this HttpClient client, string? requestUri,
       T value, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
        {
            Content = new StringContent(value!.ToJson(), Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request, cancellationToken);

        return response;
    }
}
