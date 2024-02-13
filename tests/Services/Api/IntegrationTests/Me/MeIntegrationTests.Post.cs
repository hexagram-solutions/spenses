using System.Net;
using Microsoft.EntityFrameworkCore;
using Refit;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests
{
    [Fact]
    public async Task Put_me_updates_current_user()
    {
        var props = new UserProfileProperties { DisplayName = "Torque" };

        var response = await _meApi.UpdateMe(props);

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
                DisplayName = props.DisplayName
            },
            opts => opts.Excluding(u => u.AvatarUrl));
    }

    [Fact]
    public async Task Put_me_with_invalid_properties_yields_bad_request()
    {
        var props = new UserProfileProperties { DisplayName = string.Empty };

        var response = await _meApi.UpdateMe(props);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Error!.GetContentAsAsync<ProblemDetails>();

        problemDetails!.Errors.Should().ContainKey(nameof(UserProfileProperties.DisplayName));
    }
}
