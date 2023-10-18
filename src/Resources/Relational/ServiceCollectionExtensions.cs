using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Resources.Relational.Infrastructure;

namespace Spenses.Resources.Relational;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRelationalServices(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlServer(connectionString,
                sqlOpts => sqlOpts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        return services;
    }
}
