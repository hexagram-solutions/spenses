using Hexagrams.Extensions.Common.Http;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Polly;
using Refit;
using Spenses.App.Identity;
using Spenses.Application.Common;
using Spenses.Client.Http;

namespace Spenses.App;

public static class ProgramExtensions
{
    internal static WebAssemblyHostBuilder AddIdentityServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

        builder.Services.AddScoped(
            sp => (IAccountManagement) sp.GetRequiredService<AuthenticationStateProvider>());

        return builder;
    }

    public static WebAssemblyHostBuilder AddApiClients(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<CookieHandler>();

        var retry = false;

        if (builder.HostEnvironment.IsDevelopment())
        {
            builder.Services.AddTransient(_ => new DelayingHttpHandler(TimeSpan.FromMicroseconds(500)));

            retry = true;
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

            if (retry)
            {
                clientBuilder.AddTransientHttpErrorPolicy(policy =>
                    policy.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(1.5, attempt))));
            }

            if (builder.HostEnvironment.IsDevelopment())
                clientBuilder.AddHttpMessageHandler<DelayingHttpHandler>();
        }

        AddApiClient<IAuthApi>();
        AddApiClient<IExpenseCategoriesApi>();
        AddApiClient<IExpensesApi>();
        AddApiClient<IHomesApi>();
        AddApiClient<IInsightsApi>();
        AddApiClient<IMeApi>();
        AddApiClient<IMembersApi>();
        AddApiClient<IPaymentsApi>();

        return builder;
    }
}
