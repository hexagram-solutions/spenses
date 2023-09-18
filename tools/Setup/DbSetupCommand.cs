using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spenses.Common.Extensions;
using Spenses.Resources.Relational;
using Spenses.Tools.Setup.SeedData;

namespace Spenses.Tools.Setup;

public class DbSetupCommand : RootCommand
{
    private static readonly Option<string> ConnectionOption = new("--connection",
        "The connection string to the database instead of the default supplied in appsettings.json.");

    private readonly DbContextOptionsFactory _dbContextOptionsFactory;
    private readonly ILogger<DbSetupCommand> _logger;
    private readonly IEnumerable<ISeedDataTask> _seedDataTasks;

    public DbSetupCommand(DbContextOptionsFactory dbContextOptionsFactory, ILogger<DbSetupCommand> logger,
        IEnumerable<ISeedDataTask> seedDataTasks)
        : base("Perform database setup tasks")
    {
        _logger = logger;
        _seedDataTasks = seedDataTasks;
        _dbContextOptionsFactory = dbContextOptionsFactory;

        Add(GetSeedDatabaseCommand());
        Add(GetResetDatabaseCommand());

        AddGlobalOption(ConnectionOption);
    }

    private Command GetSeedDatabaseCommand()
    {
        var seedCommand = new Command("seed", "Seed the target database with test data.");

        seedCommand.SetHandler(SeedDatabase, ConnectionOption);

        return seedCommand;
    }

    private Command GetResetDatabaseCommand()
    {
        var resetDatabaseCommand = new Command("reset", "Reset the target database by dropping all tables and views, " +
            "applying all migrations, replacing all views, and seeding the database with data.");

        var forceOption = new Option<bool>(
            new[] { "--force", "-f" },
            description: "Bypass confirmation before resetting the database.",
            getDefaultValue: () => false);

        resetDatabaseCommand.SetHandler(async (connection, force) =>
        {
            if (!await DropDatabase(connection, force))
                return;

            await MigrateDatabase(connection);
            await SeedDatabase(connection);

            _logger.LogInformation("Done resetting database!");
        }, ConnectionOption, forceOption);

        resetDatabaseCommand.AddOption(forceOption);

        return resetDatabaseCommand;
    }

    private async Task SeedDatabase(string connection)
    {
        await using var db = CreateDbContext(connection);

        _logger.LogInformation("Seeding database...");

        await _seedDataTasks
            .OrderBy(t => t.Order)
            .ForEachAsync(t =>
            {
                _logger.LogInformation("Executing seed task: {TaskName}", t.GetType().Name);

                return t.SeedData(db);
            });

        _logger.LogInformation("Database seeded.");
    }

    public async Task<bool> DropDatabase(string connection, bool force)
    {
        await using var db = CreateDbContext(connection);
        await using var dbConnection = db.Database.GetDbConnection();

        if (!force && !Confirm($"Really drop '{dbConnection.DataSource}/{dbConnection.Database}'?"))
            return false;

        _logger.LogWarning("Dropping database... ");

        await db.Database.EnsureDeletedAsync();

        _logger.LogInformation("Database dropped.");

        return true;
    }

    private async Task MigrateDatabase(string connection)
    {
        await using var db = CreateDbContext(connection);

        _logger.LogWarning("Migrating database...");

        await db.Database.MigrateAsync();

        _logger.LogInformation("Database migrated.");
    }

    private ApplicationDbContext CreateDbContext(string? connection)
    {
        return new ApplicationDbContext(_dbContextOptionsFactory.Create(connection));
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
