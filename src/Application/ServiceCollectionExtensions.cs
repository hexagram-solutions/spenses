using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Application.Features.Identity.Validators;
using Spenses.Application.Services.Invitations;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper((sp, cfg) =>
        {
            cfg.ConstructServicesUsing(sp.GetRequiredService);
        }, AppDomain.CurrentDomain.GetAssemblies());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<HomeQuery>();
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestAuthorizationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceLoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
        });

        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        services.AddTransient<InvitationTokenProvider>();

        return services;
    }
}
