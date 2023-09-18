
using Hexagrams.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spenses.Application.Common;
using Spenses.Resources.Relational;

namespace Spenses.Tools.Setup;

public class DbContextOptionsFactory
{
    private readonly IConfiguration _configuration;

    public DbContextOptionsFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DbContextOptions<ApplicationDbContext> Create(string? connection, Action<string>? logInformation = null)
    {
        connection ??= _configuration.Require(ConfigConstants.SqlServerConnectionString);

        if (logInformation != null)
        {
            var builder = new SqlConnectionStringBuilder(connection);
            logInformation($"Connecting to '{builder.DataSource}/{builder.InitialCatalog}'...");
        }

        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .LogTo(Console.WriteLine, LogLevel.Warning)
            .UseSqlServer(connection)
            .Options;
    }
}
