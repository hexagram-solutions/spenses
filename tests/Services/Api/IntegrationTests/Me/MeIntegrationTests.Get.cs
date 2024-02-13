using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests
{
    [Fact]
    public async Task Get_me_returns_current_user()
    {
        var response = await _meApi.GetMe();

        var applicationUser = new ApplicationUser();

        await DatabaseFixture.ExecuteDbContextAction(async db =>
        {
            applicationUser = await db.Users.SingleAsync(u => u.Email == response.Content!.Email);
        });

        response.Content!.Should().BeEquivalentTo(
            new CurrentUser
            {
                Id = applicationUser!.Id,
                Email = applicationUser.Email!,
                EmailVerified = applicationUser.EmailConfirmed,
                DisplayName = applicationUser.DisplayName
            },
            opts => opts.Excluding(u => u.AvatarUrl));
    }
}
