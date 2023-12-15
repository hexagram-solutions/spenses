using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class UsersSeedDataTask(IUserStore<ApplicationUser> users) : ISeedDataTask
{
    public int Order => 0;

    public async Task SeedData(ApplicationDbContext db)
    {
        Task<IdentityResult> AddUser(string id, string email)
        {
            var user = new ApplicationUser
            {
                Id = id,
                UserName = email,
                Email = email,
                NormalizedUserName = email.ToUpperInvariant(),
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true
            };

            var hashedPassword = new PasswordHasher<ApplicationUser>().HashPassword(user, "Password1!");

            user.PasswordHash = hashedPassword;

            return users.CreateAsync(user, CancellationToken.None);
        }

        await Task.WhenAll(
            AddUser(SystemCurrentUserService.SystemUserId, "system@spenses.ca"),
            AddUser(Guid.NewGuid().ToString(), "george@vandelayindustries.com"),
            AddUser(Guid.NewGuid().ToString(), "ericsondergard+spensesuser@fastmail.com"));
    }
}
