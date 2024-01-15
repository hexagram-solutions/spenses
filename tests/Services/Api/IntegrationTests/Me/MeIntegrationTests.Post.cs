using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests
{
    [Fact]
    public async Task Put_me_updates_current_user()
    {
        var props = new UserProfileProperties { DisplayName = "Torque" };

        var response = await _meApi.UpdateMe(props);

        var currentUserService = fixture.WebApplicationFactory.Services.GetRequiredService<ICurrentUserService>();
        var userManager = fixture.WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        var applicationUser = await userManager.GetUserAsync(currentUserService.CurrentUser!);

        response.Content!.Should().BeEquivalentTo(new CurrentUser
        {
            Email = applicationUser!.Email!,
            EmailVerified = applicationUser.EmailConfirmed,
            DisplayName = props.DisplayName
        });
    }

    [Fact]
    public async Task Put_me_with_invalid_properties_yields_bad_request()
    {
        var props = new UserProfileProperties { DisplayName = string.Empty };

        var response = await _meApi.UpdateMe(props);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails.Errors.Should().ContainKey(nameof(UserProfileProperties.DisplayName));
    }
}
