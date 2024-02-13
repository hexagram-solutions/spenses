using System.Net;
using Bogus;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Homes;

public partial class HomesIntegrationTests
{
    [Fact]
    public async Task Get_non_existent_home_yields_not_found_result()
    {
        var result = await _homes.GetHome(Guid.NewGuid());

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_home_where_current_user_is_not_a_member_returns_unauthorized()
    {
        var createdHomeResponse = await _homes.PostHome(
            new HomeProperties { Name = "Our House", Description = "Is in the middle of our street" });

        var registerRequest = new RegisterRequest
        {
            DisplayName = "Cartoons Plural",
            Email = "cartoons.plural@vt.edu",
            Password = new Faker().Internet.Password()
        };

        await AuthFixture.Register(registerRequest, true);
        await AuthFixture.Login(new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        var homeResponse = await _homes.GetHome(createdHomeResponse.Content!.Id);

        homeResponse.Error!.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
