using System.Reflection;
using Blazorise.Bootstrap5;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using BlazorState;
using FluentValidation;
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

        AddApiClient<IHomesApi>();
        AddApiClient<IMembersApi>();
        AddApiClient<IExpensesApi>();
        AddApiClient<IPaymentsApi>();

        return services;
    }

    public static IServiceCollection AddStateManagement(this IServiceCollection services, bool useDevTools)
    {
        services.AddBlazorState(options =>
        {
            if (useDevTools)
                options.UseReduxDevTools();

            options.Assemblies = new[] { typeof(Program).GetTypeInfo().Assembly };
        });

        return services;
    }

    public static IServiceCollection AddBlazoriseComponents(this IServiceCollection services)
    {
        services
            .AddBlazorise()
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            .AddBlazoriseFluentValidation();

        // todo: is there a better hook for this?
        services.AddValidatorsFromAssemblyContaining<HomePropertiesValidator>();

        return services;
    }
}
