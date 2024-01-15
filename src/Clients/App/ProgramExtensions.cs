using Fluxor;
using Hexagrams.Extensions.Common.Http;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Polly;
using Refit;
using Spenses.App.Authentication;
using Spenses.App.Infrastructure;
using Spenses.Client.Http;
using Spenses.Shared.Common;
using Spenses.Utilities.Security;

namespace Spenses.App;

internal static class ProgramExtensions
{
    internal static WebAssemblyHostBuilder AddWebServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services
            .AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
            .AddMudServices();

        return builder;
    }

    internal static WebAssemblyHostBuilder AddIdentityServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore(configure =>
        {
            configure.AddPolicy(AuthorizationConstants.RequireVerifiedEmail, policy =>
                policy.RequireClaim(ApplicationClaimTypes.EmailVerified, "true".ToLowerInvariant()));
        });

        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

        builder.Services.AddScoped(
            sp => (IAuthenticationService) sp.GetRequiredService<AuthenticationStateProvider>());

        return builder;
    }

    internal static WebAssemblyHostBuilder AddApiClients(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<CookieHandler>();

        if (builder.HostEnvironment.IsDevelopment())
        {
            builder.Services.AddTransient(_ => new DelayingHttpHandler(TimeSpan.FromMicroseconds(500)));
        }

        var baseUrl = builder.Configuration.Require(ConfigConstants.SpensesApiBaseUrl);

        void AddApiClient<T>()
            where T : class
        {
            var clientBuilder = builder.Services
                .AddRefitClient<T>(_ => new RefitSettings
                {
                    CollectionFormat = CollectionFormat.Multi
                })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<CookieHandler>();

            if (!builder.HostEnvironment.IsDevelopment())
                clientBuilder.AddStandardResilienceHandler();
            else
                clientBuilder.AddHttpMessageHandler<DelayingHttpHandler>();
        }

        AddApiClient<IIdentityApi>();
        AddApiClient<IExpenseCategoriesApi>();
        AddApiClient<IExpensesApi>();
        AddApiClient<IHomesApi>();
        AddApiClient<IInsightsApi>();
        AddApiClient<IMeApi>();
        AddApiClient<IMembersApi>();
        AddApiClient<IPaymentsApi>();

        return builder;
    }

    internal static WebAssemblyHostBuilder AddStateManagement(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddFluxor(opts =>
        {
            opts.ScanAssemblies(typeof(Program).Assembly)
                .UseRouting();

            if (builder.HostEnvironment.IsEnvironment(EnvironmentNames.Development))
                opts.UseReduxDevTools();
        });

        return builder;
    }
}
