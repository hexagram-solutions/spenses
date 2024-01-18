using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests
{
    [Fact]
    public async Task Get_me_returns_current_user()
    {
        var response = await _meApi.GetMe();

        var currentUserService = fixture.WebApplicationFactory.Services.GetRequiredService<ICurrentUserService>();
        var userManager = fixture.WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        var applicationUser = await userManager.GetUserAsync(currentUserService.CurrentUser!);

        response.Content!.Should().BeEquivalentTo(new CurrentUser
        {
            Email = applicationUser!.Email!,
            EmailVerified = applicationUser.EmailConfirmed,
            DisplayName = applicationUser.DisplayName
        });
    }
}
