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
                Members = userIdentities.Select(u => new Member
                {
                    Name = u.DisplayName,
                    DefaultSplitPercentage = 1m / userIdentities.Count,
                    Status = MemberStatus.Active,
                    UserId = u.Id
                }).ToList(),
                ExpenseCategories =
                {
                    new ExpenseCategory
                    {
                        Name = "General (uncategorized)",
                        IsDefault = true
                    },
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
                Members = userIdentities.Select(u => new Member
                {
                    Name = u.UserName!,
                    DefaultSplitPercentage = 1m / userIdentities.Count,
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
