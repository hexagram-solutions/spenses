using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class UsersSeedDataTask(IUserStore<ApplicationUser> users) : ISeedDataTask
{
    public int Order => 0;

    public async Task SeedData(ApplicationDbContext db)
    {
        await db.Users.AddAsync(new ApplicationUser
        {
            Id = SystemCurrentUserService.SystemUserId,
            Email = "system@spenses.ca",
            UserName = "George Costanza",
        });

        await db.Users.AddAsync(new ApplicationUser
        {
            Id = "integration-test-user",
            Email = "george@vandelayindustries.com",
            UserName = "George Costanza",
        });

        await db.SaveChangesAsync();

        var user = new ApplicationUser
        {
            UserName = "Eric Sondergard",
            Email = "ericsondergard+spensesuser@fastmail.com",
            NormalizedUserName = "ericsondergard+spensesuser@fastmail.com".ToUpperInvariant(),
            NormalizedEmail = "ericsondergard+spensesuser@fastmail.com".ToUpperInvariant(),
            EmailConfirmed = true
        };

        var hashedPassword = new PasswordHasher<ApplicationUser>().HashPassword(user, "Password1!");

        user.PasswordHash = hashedPassword;

        await users.CreateAsync(user, CancellationToken.None);
    }
}
