using System.CommandLine;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
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
    private readonly ICurrentUserService _currentUserService;

    public DbSetupCommand(DbContextOptionsFactory dbContextOptionsFactory, ILogger<DbSetupCommand> logger,
        IEnumerable<ISeedDataTask> seedDataTasks, ICurrentUserService currentUserService)
        : base("Perform database setup tasks")
    {
        _logger = logger;
        _seedDataTasks = seedDataTasks;
        _currentUserService = currentUserService;
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
            await RebuildViews(connection);
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

        foreach (var seedDataTask in _seedDataTasks.OrderBy(t => t.Order))
        {
            _logger.LogInformation("Executing seed task: {TaskName}", seedDataTask.GetType().Name);

            await seedDataTask.SeedData(db);
        }

        _logger.LogInformation("Database seeded.");
    }

    public async Task<bool> DropDatabase(string connection, bool force)
    {
        await using var dbContext = CreateDbContext(connection);
        await using var dbConnection = dbContext.Database.GetDbConnection();

        if (!force && !Confirm($"Really drop '{dbConnection.DataSource}/{dbConnection.Database}'?"))
            return false;

        var db = GetSqlServerDatabase(dbConnection);

        _logger.LogWarning("Dropping tables...");

        var sql = string.Empty;

        foreach (Table? table in db.Tables)
        {
            sql = table!.ForeignKeys.Cast<ForeignKey?>().Aggregate(sql,
                (current, fk) => $"ALTER TABLE [{table.Name}] DROP CONSTRAINT [{fk!.Name}];\n" + current);

            sql += $"DROP TABLE [{table.Name}];\n";
        }
        db.ExecuteNonQuery(sql);

        _logger.LogWarning("Dropping views...");

        foreach (var view in db.Views.Cast<View>().Where(v => !v.IsSystemObject))
        {
            db.ExecuteNonQuery($"DROP VIEW [{view!.Name}]");
        }

        _logger.LogWarning("Dropping sequences...");

        foreach (var sequence in db.Sequences.Cast<Sequence>())
        {
            db.ExecuteNonQuery($"DROP SEQUENCE [{sequence!.Name}]");
        }

        return true;
    }

    private static Database GetSqlServerDatabase(DbConnection dbConnection)
    {
        var server = new Server(new ServerConnection((SqlConnection) dbConnection));

        server.Refresh();

        return server.Databases[dbConnection.Database];
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

        db.UpdateViews();

        _logger.LogInformation("Done rebuilding views.");
    }

    private ApplicationDbContext CreateDbContext(string? connection)
    {
        return new ApplicationDbContext(_dbContextOptionsFactory.Create(connection),
            new AuditableEntitySaveChangesInterceptor(_currentUserService));
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
