using System.Net;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Logout_unauthenticated_request_yields_success()
    {
        var logoutResponse = await AuthFixture.Logout();

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
