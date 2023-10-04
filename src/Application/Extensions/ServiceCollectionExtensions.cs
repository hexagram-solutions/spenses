using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Application.Features.Homes.Validators;

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
        }, userCodeAssemblies);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<HomeQuery>();
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestAuthorizationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceLoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
        });

        services.AddValidatorsFromAssemblyContaining<CreateHomeCommandValidator>();

        return services;
    }
}
