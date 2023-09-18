using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class HomesSeedDataTask : ISeedDataTask
{
    public int Order => 10;

    public async Task SeedData(ApplicationDbContext db)
    {
        await db.Homes.AddAsync(new Home
        {
            Name = "Test home",
            Description = "Test home for integration testing",
            Members =
            {
                new Member
                {
                    Name = "Alice",
                    AnnualTakeHomeIncome = 80_000m
                },
                new Member
                {
                    Name = "Bob",
                    AnnualTakeHomeIncome = 72_000m
                }
            }
        });

        await db.SaveChangesAsync();
    }
}
