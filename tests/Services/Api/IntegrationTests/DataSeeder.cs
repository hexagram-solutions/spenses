using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spenses.Resources.Relational;
using Spenses.Tools.Setup.SeedData;

namespace Spenses.Api.IntegrationTests;

public class DataSeeder(ApplicationDbContext db, IEnumerable<ISeedDataTask> tasks, ILogger<DataSeeder> logger)
{
    public async Task SeedDatabase()
    {
        logger.LogDebug("Seeding database...");

        foreach (var seedDataTask in tasks.OrderBy(t => t.Order))
        {
            logger.LogDebug("Executing seed task: {TaskName}", seedDataTask.GetType().Name);

            await seedDataTask.SeedData(db);
        }

        logger.LogDebug("Database seeded.");
    }
}

public static class DataSeedingServiceCollectionExtensions
{
    public static IServiceCollection AddDataSeeder(this IServiceCollection services)
    {
        services.AddTransient<DataSeeder>();
        services.Scan(scan => scan
            .FromAssemblyOf<ISeedDataTask>()
            .AddClasses(classes => classes.AssignableTo<ISeedDataTask>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
