using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;

namespace Spenses.Tools.Setup.SeedData;

public class UsersSeedDataTask(
    UserManager<ApplicationUser> innerUserManager,
    IConfiguration configuration,
    IServiceProvider services,
    ILogger<UserManager<ApplicationUser>> userManagerLogger,
    IOptions<IdentityOptions> identityOptions) : ISeedDataTask
{
    public int Order => 0;

    public async Task SeedData(ApplicationDbContext db)
    {
        // Sub in the DB context to make sure that the target database configured by command-line args is used
        using var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(db),
            identityOptions,
            innerUserManager.PasswordHasher,
            innerUserManager.UserValidators,
            innerUserManager.PasswordValidators,
            innerUserManager.KeyNormalizer,
            innerUserManager.ErrorDescriber,
            services,
            userManagerLogger);

        const string defaultUserPasswordSettingKey = "DefaultUserPassword";

        var defaultPassword = configuration.Require(defaultUserPasswordSettingKey,
            $"A value for {defaultUserPasswordSettingKey} must be set in user secrets.");

        await AddUser(SystemCurrentUserService.SystemUserId, "system@spenses.ca", "System User");
        await AddUser("integration-test-user", "george@vandelayindustries.com", "George Costanza");
        await AddUser(Guid.NewGuid().ToString(), "ericsondergard+spensesuser@fastmail.com", "esond");

        return;

        Task<IdentityResult> AddUser(string id, string email, string displayName)
        {
            var user = new ApplicationUser
            {
                Id = id,
                UserName = email,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true
            };

            return userManager.CreateAsync(user, defaultPassword);
        }
    }
}
