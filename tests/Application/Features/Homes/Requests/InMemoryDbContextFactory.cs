using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Infrastructure;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Tests.Features.Homes.Requests;

public class InMemoryDbContextFactory : IDbContextFactory
{
    private readonly IUserContext _userContext;

    private readonly SqliteConnection _connection;

    public InMemoryDbContextFactory(IUserContext userContext)
    {
        _userContext = userContext;

        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    public ApplicationDbContext Create()
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        var db = new ApplicationDbContext(dbOptions,
            new AuditableEntitySaveChangesInterceptor(_userContext));

        db.Database.EnsureCreated();

        return db;
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}
