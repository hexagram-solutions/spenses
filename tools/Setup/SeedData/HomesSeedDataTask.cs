using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class HomesSeedDataTask : ISeedDataTask
{
    public int Order => 10;

    public async Task SeedData(ApplicationDbContext db)
    {
        var userIdentities = await db.Users.ToListAsync();

        await db.Homes.AddRangeAsync(
            new Home
            {
                Name = "Test home",
                Description = "Test home for integration testing",
                ExpensePeriod = ExpensePeriod.Monthly,
                Members = userIdentities.Select(u => new Member
                {
                    Name = u.NickName,
                    DefaultSplitPercentage = 1d / userIdentities.Count,
                    UserId = u.Id
                }).ToList(),
                ExpenseCategories =
                {
                    new ExpenseCategory
                    {
                        Name = "groceries",
                    },
                    new ExpenseCategory
                    {
                        Name = "housing"
                    }
                }
            },
            new Home
            {
                Name = "Test home 2",
                Description = "Second test home",
                ExpensePeriod = ExpensePeriod.Weekly,
                Members = userIdentities.Select(u => new Member
                {
                    Name = u.NickName,
                    DefaultSplitPercentage = 1d / userIdentities.Count,
                    UserId = u.Id
                }).ToList(),
                ExpenseCategories =
                {
                    new ExpenseCategory
                    {
                        Name = "groceries",
                    },
                    new ExpenseCategory
                    {
                        Name = "housing"
                    }
                }
            });

        await db.SaveChangesAsync();
    }
}
