using Bogus;
using Hexagrams.Extensions.Common;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class ExpensesSeedDataTask : ISeedDataTask
{
    public int Order => 20;

    public async Task SeedData(ApplicationDbContext db)
    {
        var faker = new Faker();

        var homes = await db.Homes
            .Include(h => h.Members)
            .ToListAsync();

        foreach (var home in homes)
        {
            for (var i = 0; i < 10; i++)
            {
                home.Expenses.Add(new Expense
                {
                    Description = faker.Lorem.Sentence(3),
                    Date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(Random.Shared.Next(-10, 10)),
                    Amount = Random.Shared.NextDecimal(5, 500, 2),
                    IncurredByMember = Random.Shared.NextItem(home.Members)
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
