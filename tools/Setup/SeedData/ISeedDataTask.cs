using Spenses.Resources
    .Relational;

namespace Spenses.Tools.Setup.SeedData;

public interface ISeedDataTask
{
    int Order { get; }

    Task SeedData(ApplicationDbContext db);
}
