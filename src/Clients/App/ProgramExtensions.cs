using FluentValidation;
using Fluxor;
using Hexagrams.Extensions.Common.Http;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Morris.Blazor.Validation;
using MudBlazor;
using MudBlazor.Services;
using Refit;
using Spenses.App.Infrastructure.Authentication;
using Spenses.Client.Http;
using Spenses.Shared.Common;
using Spenses.Shared.Validators.Homes;
using Spenses.Shared.Validators.Identity;
using Spenses.Utilities.Security;

namespace Spenses.App;

internal static class ProgramExtensions
{
    internal static WebAssemblyHostBuilder AddWebServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services
            .AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
            .AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            });

        builder.Services.AddFormValidation(config =>
            config.AddFluentValidation(typeof(HomePropertiesValidator).Assembly));

        builder.Services.AddValidatorsFromAssemblyContaining(typeof(LoginRequestValidator));

        builder.Services.AddLocalization();

        if (!builder.HostEnvironment.IsDevelopment())
            builder.Logging.SetMinimumLevel(LogLevel.Warning);

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
            builder.Services.AddScoped(_ => new DelayingHttpHandler(TimeSpan.FromMilliseconds(500)));

        var baseUrl = builder.Configuration.Require(ConfigConstants.SpensesApiBaseUrl);

        AddApiClient<IExpenseCategoriesApi>();
        AddApiClient<IExpensesApi>();
        AddApiClient<IHomesApi>();
        AddApiClient<IIdentityApi>();
        AddApiClient<IInsightsApi>();
        AddApiClient<IInvitationsApi>();
        AddApiClient<IMeApi>();
        AddApiClient<IMembersApi>();
        AddApiClient<IPaymentsApi>();

        return builder;

        void AddApiClient<T>()
            where T : class
        {
            var clientBuilder = builder.Services
                .AddRefitClient<T>(_ => new RefitSettings { CollectionFormat = CollectionFormat.Multi })
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl))
                .AddHttpMessageHandler<CookieHandler>();

            if (builder.HostEnvironment.IsDevelopment())
                clientBuilder.AddHttpMessageHandler<DelayingHttpHandler>();
            else
                clientBuilder.AddStandardResilienceHandler();
        }
    }

    internal static WebAssemblyHostBuilder AddStateManagement(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddFluxor(opts =>
        {
            opts.ScanAssemblies(typeof(Program).Assembly)
                .UseRouting();

            if (builder.HostEnvironment.IsDevelopment())
                opts.UseReduxDevTools();
        });

        return builder;
    }
}
