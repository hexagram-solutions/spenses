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
                    Name = u.NickName,
                    UserId = u.Id
                }).ToList()
            },
            new Home
            {
                Name = "Test home 2",
                Description = "Second test home",
                Members = userIdentities.Select(u => new Member
                {
                    Name = u.NickName,
                    UserId = u.Id
                }).ToList()
            });

        await db.SaveChangesAsync();
    }
}
