using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class UserIdentitiesSeedDataTask : ISeedDataTask
{
    public int Order => 0;

    public async Task SeedData(ApplicationDbContext db)
    {
        await db.Users.AddAsync(new UserIdentity
        {
            Id = SystemCurrentUserService.SystemUserId,
            Email = "system@spenses.ca",
            Name = "System User",
            Issuer = "self"
        });

        await db.Users.AddAsync(new UserIdentity
        {
            Id = "integration-test-user",
            Email = "george@vandelayindustries.com",
            Name = "George Costanza",
            Issuer = "self"
        });

        await db.Users.AddAsync(new UserIdentity
        {
            Id = "auth0|64ad7b9ec359b5dc9d467a5b",
            Name = "ericsondergard+spensesuser@fastmail.com",
            Issuer = "https://spenses.us.auth0.com/",
            Email = "ericsondergard+spensesuser@fastmail.com"
        });

        await db.SaveChangesAsync();
    }
}
