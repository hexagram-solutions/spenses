using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class UsersSeedDataTask(UserManager<ApplicationUser> users, IConfiguration configuration) : ISeedDataTask
{
    public int Order => 0;

    public async Task SeedData(ApplicationDbContext db)
    {
        const string defaultUserPasswordSettingKey = "DefaultUserPassword";

        var defaultPassword = configuration.Require(defaultUserPasswordSettingKey,
            $"A value for {defaultUserPasswordSettingKey} must be set in user secrets.");

        await AddUser(SystemCurrentUserService.SystemUserId, "system@spenses.ca");
        await AddUser("integration-test-user", "george@vandelayindustries.com");
        await AddUser(Guid.NewGuid().ToString(), "ericsondergard+spensesuser@fastmail.com");

        return;

        Task<IdentityResult> AddUser(string id, string email)
        {
            var user = new ApplicationUser
            {
                Id = id,
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            return users.CreateAsync(user, defaultPassword);
        }
    }
}
