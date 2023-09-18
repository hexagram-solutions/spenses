using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Features.Homes;
using Spenses.Common.Extensions;

namespace Spenses.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var userCodeAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name!.StartsWith("Spenses"))
            .ToArray(); // todo: why doesn't Spenses.Application show up in assys?

        services.AddAutoMapper((sp, cfg) =>
        {
            cfg.ConstructServicesUsing(sp.GetRequiredService);
            //}, userCodeAssemblies);
        }, typeof(HomeQuery).Yield());

        services.AddAutoMapper((sp, cfg) =>
        {
            cfg.ConstructServicesUsing(sp.GetRequiredService);
            //}, userCodeAssemblies);
        }, typeof(HomeQuery).Yield());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<HomeQuery>());

        return services;
    }
}
