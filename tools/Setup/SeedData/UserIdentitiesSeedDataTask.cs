using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class UserIdentitiesSeedDataTask : ISeedDataTask
{
    public int Order => 0;

    public async Task SeedData(ApplicationDbContext db)
    {
        var systemUser = await db.FindAsync<UserIdentity>(SystemCurrentUserService.SystemUserId);

        if (systemUser is not null)
            return;

        await db.AddAsync(new UserIdentity
        {
            Id = SystemCurrentUserService.SystemUserId,
            Email = "system@spenses.ca",
            Name = "System User",
            Issuer = "self"
        });

        await db.SaveChangesAsync();
    }
}
