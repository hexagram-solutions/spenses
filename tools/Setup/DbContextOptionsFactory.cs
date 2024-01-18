using Hexagrams.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spenses.Resources.Relational;
using Spenses.Shared.Common;

namespace Spenses.Tools.Setup;

public class DbContextOptionsFactory(IConfiguration configuration)
{
    public DbContextOptions<ApplicationDbContext> Create(string? connection, Action<string>? logInformation = null)
    {
        connection ??= configuration.Require(ConfigConstants.SqlServerConnectionString);

        if (logInformation != null)
        {
            var builder = new SqlConnectionStringBuilder(connection);
            logInformation($"Connecting to '{builder.DataSource}/{builder.InitialCatalog}'...");
        }

        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .LogTo(Console.WriteLine, LogLevel.Warning)
            .UseSqlServer(connection, sqlOpts => sqlOpts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .Options;
    }
}
