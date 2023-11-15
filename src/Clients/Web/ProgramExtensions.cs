using Blazorise.Bootstrap5;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using FluentValidation;
using Fluxor;
using Hexagrams.Extensions.Authentication.OAuth;
using Hexagrams.Extensions.Common.Http;
using Refit;
using Spenses.Application.Features.Homes.Validators;
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

        if (delay.HasValue)
            services.AddTransient(_ => new DelayingHttpHandler(delay.Value));

        void AddApiClient<T>()
            where T : class
        {
            var clientBuilder = services
                .AddRefitClient<T>(_ => new RefitSettings
                {
                    CollectionFormat = CollectionFormat.Multi
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<BearerTokenAuthenticationHandler>();

            if (delay.HasValue)
                clientBuilder.AddHttpMessageHandler<DelayingHttpHandler>();
        }

        AddApiClient<IExpenseCategoriesApi>();
        AddApiClient<IExpensesApi>();
        AddApiClient<IHomesApi>();
        AddApiClient<IMembersApi>();
        AddApiClient<IPaymentsApi>();

        return services;
    }

    public static IServiceCollection AddStateManagement(this IServiceCollection services, bool useDevTools)
    {
        services.AddFluxor(opts =>
        {
            opts.ScanAssemblies(typeof(Program).Assembly)
                .UseRouting()
                .UseReduxDevTools();
            // TODO: redux dev tools
            // TODO: routing
        });

        return services;
    }

    public static IServiceCollection AddBlazoriseComponents(this IServiceCollection services)
    {
        services
            .AddBlazorise(opts =>
            {
                opts.Debounce = true;
                opts.ProductToken =
                    "CjxRA3B/NQs+UAVwezY1BlEAc3o0CTxSAXZ8MAg/bjoNJ2ZdYhBVCCo/DD9UPUsNalV8Al44B2ECAWllMit3cWhZPUsvbUBCIgcrLCp+RQdgGQs0D0MUXhMFTlcjVB8lI2QBfiZDIgxMTkI2CAQFfAlHBkkQN0pqaxBVNSNKe3kMBiBhdF5aEmIkPjFWPxN7FiFIUVVIdS4fYAxhTHpuO3N/YxlcJQN3TFomcSkbXX95MGcwDDEMVDJgKR53UU4SfTUPTV9cF1U2LGpvTVREKj1SUjkTVnA7d21bVwcXKnZPNA9FJHppYD8zHxAbN3kx";
            })
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            .AddBlazoriseFluentValidation();

        // todo: is there a better hook for this?
        services.AddValidatorsFromAssemblyContaining<HomePropertiesValidator>();

        return services;
    }
}
