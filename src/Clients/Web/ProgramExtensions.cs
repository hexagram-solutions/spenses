using System.Reflection;
using BlazorState;
using Hexagrams.Extensions.Authentication.OAuth;
using Hexagrams.Extensions.Common.Http;
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

    public static IServiceCollection AddApiClients(this IServiceCollection services, string baseUrl, string[] scopes,
        TimeSpan? delay = null)
    {
        services.AddAccessTokenProvider(options =>
        {
            options.UseCustomProvider<WebAssemblyAuthenticationAccessTokenProvider>()
                .WithInMemoryCaching();
        });

        services.AddTransient(sp => new BearerTokenAuthenticationHandler(
            sp.GetRequiredService<IAccessTokenProvider>(), scopes));

        var clientBuilder = services
            .AddRefitClient<IHomesApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<BearerTokenAuthenticationHandler>();

        if (delay is not null)
        {
            services.AddTransient(_ => new DelayingHttpHandler(TimeSpan.FromMilliseconds(500)));
            clientBuilder.AddHttpMessageHandler<DelayingHttpHandler>();
        }

        return services;
    }

    public static IServiceCollection AddStateManagement(this IServiceCollection services)
    {
        services.AddBlazorState(options => options.Assemblies = new[] { typeof(Program).GetTypeInfo().Assembly });

        return services;
    }
}
