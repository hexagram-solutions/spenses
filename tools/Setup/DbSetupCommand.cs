using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Respawn;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Infrastructure;
using Spenses.Tools.Setup.SeedData;
using Spenses.Utilities.Security.Services;

namespace Spenses.Tools.Setup;

public class DbSetupCommand : RootCommand
{
    private static readonly Option<string> ConnectionOption = new("--connection",
        "The connection string to the database instead of the default supplied in appsettings.json.");

    private readonly DbContextOptionsFactory _dbContextOptionsFactory;
    private readonly ILogger<DbSetupCommand> _logger;
    private readonly IEnumerable<ISeedDataTask> _seedDataTasks;
    private readonly IUserContext _userContext;

    public DbSetupCommand(DbContextOptionsFactory dbContextOptionsFactory, ILogger<DbSetupCommand> logger,
        IEnumerable<ISeedDataTask> seedDataTasks, IUserContext userContext)
        : base("Perform database setup tasks")
    {
        _logger = logger;
        _seedDataTasks = seedDataTasks;
        _userContext = userContext;
        _dbContextOptionsFactory = dbContextOptionsFactory;

        Add(GetSeedDatabaseCommand());
        Add(GetRebuildViewsCommand());
        Add(GetResetDatabaseCommand());

        AddGlobalOption(ConnectionOption);
    }

    private Command GetSeedDatabaseCommand()
    {
        var seedCommand = new Command("seed", "Seed the target database with test data.");

        seedCommand.SetHandler(SeedDatabase, ConnectionOption);

        return seedCommand;
    }

    private Command GetRebuildViewsCommand()
    {
        var rebuildViewsCommand = new Command("views", "Rebuild the views in the database.");

        rebuildViewsCommand.SetHandler(RebuildViews, ConnectionOption);

        return rebuildViewsCommand;
    }

    private Command GetResetDatabaseCommand()
    {
        var resetDatabaseCommand = new Command("reset", "Reset the target database by dropping all tables and views, " +
            "applying all migrations, and rebuilding all tables and views.");

        var forceOption = new Option<bool>(["--force", "-f"],
            description: "Bypass confirmation before resetting the database.",
            getDefaultValue: () => false);

        var seedOption = new Option<bool>(["--seed", "-s"],
            description: "Seed the database with data after rebuilding.",
            getDefaultValue: () => false);

        resetDatabaseCommand.SetHandler(async (connection, force, shouldSeed) =>
        {
            await using var dbContext = CreateDbContext(connection);
            await using var dbConnection = dbContext.Database.GetDbConnection();

            if (!force && !Confirm($"Really reset '{dbConnection.DataSource}/{dbConnection.Database}'?"))
                return;

            await dbConnection.OpenAsync();

            _logger.LogWarning("Resetting tables...");

            var respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
            {
                CheckTemporalTables = true,
                TablesToIgnore = ["__EFMigrationsHistory"]
            });

            await respawner.ResetAsync(dbConnection);

            await MigrateDatabase(dbConnection.ConnectionString);
            await RebuildViews(dbConnection.ConnectionString);

            if (shouldSeed)
                await SeedDatabase(dbConnection.ConnectionString);

            _logger.LogInformation("Done resetting database!");
        }, ConnectionOption, forceOption, seedOption);

        resetDatabaseCommand.AddOption(forceOption);
        resetDatabaseCommand.AddOption(seedOption);

        return resetDatabaseCommand;
    }

    private async Task SeedDatabase(string connection)
    {
        await using var db = CreateDbContext(connection);

        _logger.LogInformation("Seeding database...");

        foreach (var seedDataTask in _seedDataTasks.OrderBy(t => t.Order))
        {
            _logger.LogInformation("Executing seed task: {TaskName}", seedDataTask.GetType().Name);

            await seedDataTask.SeedData(db);
        }

        _logger.LogInformation("Database seeded.");
    }

    private async Task MigrateDatabase(string connection)
    {
        await using var db = CreateDbContext(connection);

        _logger.LogWarning("Migrating database...");

        await db.Database.MigrateAsync();

        _logger.LogInformation("Database migrated.");
    }

    private async Task RebuildViews(string connection)
    {
        await using var db = CreateDbContext(connection);

        _logger.LogInformation("Rebuilding views... ");

        await db.UpdateViews();

        _logger.LogInformation("Done rebuilding views.");
    }

    private ApplicationDbContext CreateDbContext(string? connection)
    {
        return new ApplicationDbContext(_dbContextOptionsFactory.Create(connection),
            new AuditableEntitySaveChangesInterceptor(_userContext));
    }

    private static bool Confirm(string prompt)
    {
        Console.Write($"{prompt} ([Y]es/[N]o):");

        var response = Console.ReadLine()?.Trim().ToLowerInvariant();

        var confirmed = response == "y";

        if (!confirmed)
            Console.WriteLine("Operation aborted.");

        return confirmed;
    }
}
