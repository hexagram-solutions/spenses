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
            .Include(h => h.ExpenseCategories)
            .ToListAsync();

        var sampleTags = new[] { "internet", "restaurants", "gas", "entertainment" };

        foreach (var home in homes)
        {
            for (var i = 0; i < 50; i++)
            {
                var firstTag = Random.Shared.NextItem(sampleTags);
                var secondTag = Random.Shared.NextItem(sampleTags.Except(firstTag.Yield()));

                var expense = new Expense
                {
                    Description = faker.Lorem.Sentence(3),
                    Date = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(Random.Shared.Next(-10, 0)),
                    Amount = Random.Shared.NextDecimal(5, 500, 2),
                    PaidByMember = Random.Shared.NextItem(home.Members),
                    Category = Random.Shared.NextItem(home.ExpenseCategories),
                    Tags = { new ExpenseTag { Name = firstTag }, new ExpenseTag { Name = secondTag } }
                };

                foreach (var member in home.Members)
                {
                    expense.ExpenseShares.Add(new ExpenseShare
                    {
                        OwedByMemberId = member.Id,
                        OwedPercentage = member.DefaultSplitPercentage,
                        // Only add owing amounts for the other members; the member that paid the expense owes nothing.
                        OwedAmount = member.Id != expense.PaidByMemberId
                            ? expense.Amount * member.DefaultSplitPercentage
                            : 0.00m
                    });
                }

                home.Expenses.Add(expense);
            }
        }

        await db.SaveChangesAsync();
    }
}
