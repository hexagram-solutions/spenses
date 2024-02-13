using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Spenses.Api.Infrastructure;
using Spenses.Resources.Relational;
using Spenses.Tools.Setup;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.IntegrationTests;

// TODO: Define seed tasks
[Collection(IdentityWebApplicationCollection.CollectionName)]
public class DatabaseFixture(IdentityWebApplicationFixture appFixture)
{
    public async Task ResetDatabase()
    {
        await using var scope = appFixture.WebApplicationFactory.Services.CreateAsyncScope();

        var services = scope.ServiceProvider;

        // Swap out user context with system user for data seeding
        var userContextProvider = services.GetRequiredService<UserContextProvider>();
        userContextProvider.SetContext(new SystemUserContext(services.GetRequiredService<IConfiguration>()));

        // Wipe the database
        var db = services.GetRequiredService<ApplicationDbContext>();

        var connectionString = db.Database.GetConnectionString()!;

        var respawner = await Respawner.CreateAsync(connectionString, new RespawnerOptions
        {
            CheckTemporalTables = true
        });

        await respawner.ResetAsync(connectionString);

        // Re-seed the database
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedDatabase();

        // Re-set the user context to the one normally used in the application
        userContextProvider.SetContext(new HttpUserContext(services.GetRequiredService<IHttpContextAccessor>()));
    }

    public async Task ExecuteDbContextAction(Func<ApplicationDbContext, Task> action)
    {
        await using var scope = appFixture.WebApplicationFactory.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await action(db);
    }
}
