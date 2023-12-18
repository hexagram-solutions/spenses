using Fluxor;
using Hexagrams.Extensions.Common.Http;
using Polly;
using Refit;
using Spenses.Client.Http;
using Spenses.Web.Client.Infrastructure;
using Spenses.Web.Client.Store.Homes;
using Spenses.Web.Client.Store.Shared;

namespace Spenses.Web.Client;

public static class ProgramExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, string baseUrl, bool retry,
        TimeSpan? delay = null)
    {
        services.AddTransient<CookieHandler>();

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
                .AddHttpMessageHandler<CookieHandler>();

            if (retry)
            {
                clientBuilder.AddTransientHttpErrorPolicy(builder =>
                    builder.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(1.5, attempt))));
            }

            if (delay.HasValue)
                clientBuilder.AddHttpMessageHandler<DelayingHttpHandler>();
        }

        AddApiClient<IExpenseCategoriesApi>();
        AddApiClient<IExpensesApi>();
        AddApiClient<IHomesApi>();
        AddApiClient<IInsightsApi>();
        AddApiClient<IMembersApi>();
        AddApiClient<IPaymentsApi>();

        return services;
    }

    public static IServiceCollection AddStateManagement(this IServiceCollection services, bool useDevTools)
    {
        services.AddFluxor(opts =>
        {
            opts.ScanAssemblies(typeof(HomesState).Assembly)
                .UseRouting();

            if (useDevTools)
                opts.UseReduxDevTools();
        });

        return services;
    }
}
