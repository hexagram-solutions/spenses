using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Common;
using Spenses.Shared.Utilities;

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
            new UserStore<ApplicationUser, IdentityRole<Guid>, ApplicationDbContext, Guid>(db),
            identityOptions,
            innerUserManager.PasswordHasher,
            innerUserManager.UserValidators,
            innerUserManager.PasswordValidators,
            innerUserManager.KeyNormalizer,
            innerUserManager.ErrorDescriber,
            services,
            userManagerLogger);

        var systemUserId = configuration.Require<Guid>(ConfigConstants.SpensesTestSystemUserId);
        var testUserId = configuration.Require<Guid>(ConfigConstants.SpensesTestIntegrationTestUserId);
        var testUserEmail = configuration.Require(ConfigConstants.SpensesTestIntegrationTestUserEmail);
        var defaultPassword = configuration.Require(ConfigConstants.SpensesTestDefaultUserPassword,
            $"A value for {ConfigConstants.SpensesTestDefaultUserPassword} must be set in user secrets.");

        await AddUser(systemUserId, "system@spenses.money", "System User");
        await AddUser(testUserId, testUserEmail, "Grunky Peep");
        await AddUser(Guid.NewGuid(), "ericsondergard+spensesuser@fastmail.com", "esond");

        await db.SaveChangesAsync();

        return;

        Task<IdentityResult> AddUser(Guid id, string email, string displayName)
        {
            var user = new ApplicationUser
            {
                Id = id,
                UserName = email,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true,
                AvatarUrl = AvatarHelper.GetGravatarUri(email).ToString()
            };

            return userManager.CreateAsync(user, defaultPassword);
        }
    }
}
