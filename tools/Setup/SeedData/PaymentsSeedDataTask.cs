using Bogus;
using Hexagrams.Extensions.Common;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class PaymentsSeedDataTask : ISeedDataTask
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
            for (var i = 0; i < 25; i++)
            {
                home.Payments.Add(new Payment
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(Random.Shared.Next(-10, 0)),
                    Amount = Random.Shared.NextDecimal(5, 200, 2),
                    PaidByMember = Random.Shared.NextItem(home.Members),
                    Note = faker.Lorem.Sentence()
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
