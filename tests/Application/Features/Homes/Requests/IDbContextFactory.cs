using Spenses.Resources.Relational;

namespace Spenses.Application.Tests.Features.Homes.Requests;

public interface IDbContextFactory : IAsyncDisposable
{
    ApplicationDbContext Create();
}
