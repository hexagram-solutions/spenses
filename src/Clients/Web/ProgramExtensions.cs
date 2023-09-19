using Hexagrams.Extensions.Authentication.OAuth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Refit;
using Spenses.Client.Http;
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
            options.UseCustomProvider<WebAssemblyAuthenticationTokenProvider>()
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

public class WebAssemblyAuthenticationTokenProvider : IAccessTokenProvider
{
    private readonly IAccessTokenProviderAccessor _accessor;

    public WebAssemblyAuthenticationTokenProvider(IAccessTokenProviderAccessor accessor)
    {
        _accessor = accessor;
    }

    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var tokenResult = await _accessor.TokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = scopes });

        tokenResult.TryGetToken(out var accessToken);

        var utcNow = DateTimeOffset.UtcNow;

        return new AccessTokenResponse
        {
            AccessToken = accessToken.Value,
            ExpiresIn = (int) accessToken.Expires.Subtract(utcNow).TotalSeconds,
            Scope = string.Join(' ', accessToken.GrantedScopes),
        };
    }
}

public static class CustomAccessTokenProviderBuilderExtensions
{
    public static IAccessTokenProviderBuilder UseCustomProvider<TProvider>(this IAccessTokenProviderBuilder builder)
        where TProvider : class, IAccessTokenProvider
    {
        builder.Services.AddTransient<IAccessTokenProvider, TProvider>();

        return builder;
    }
}
