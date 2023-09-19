using Hexagrams.Extensions.Authentication.OAuth;
using Refit;
using Spenses.Client.Http;
using Spenses.Client.Web.Infrastructure;
using IAccessTokenProvider = Hexagrams.Extensions.Authentication.OAuth.IAccessTokenProvider;

namespace Spenses.Client.Web;

public static class ProgramExtensions
{
    public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, string authority,
        string clientId, string audience)
    {
        services.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.Authority = authority;
            options.ProviderOptions.ClientId = clientId;
            options.ProviderOptions.AdditionalProviderParameters.Add("audience", audience);

            options.ProviderOptions.ResponseType = "code";
        });

        return services;
    }

    public static IServiceCollection AddApiClients(this IServiceCollection services, string baseUrl, string[] scopes)
    {
        services.AddAccessTokenProvider(options =>
        {
            options.UseCustomProvider<WebAssemblyAuthenticationAccessTokenProvider>()
                .WithInMemoryCaching();
        });

        services.AddTransient(sp => new BearerTokenAuthenticationHandler(
            sp.GetRequiredService<IAccessTokenProvider>(), scopes));

        services
            .AddRefitClient<IHomesApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<BearerTokenAuthenticationHandler>();

        return services;
    }
}
