using System.Net;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Verify_email_using_parameters_from_email_sets_email_as_verified()
    {
        var registerRequest = new RegisterRequest
        {
            DisplayName = "Quatro Quatro",
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password()
        };

        await AuthFixture.Register(registerRequest);

        var response = await AuthFixture.VerifyUser(registerRequest.Email);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Verify_email_with_invalid_parameters_yields_bad_request()
    {
        var response = await _identityApi.VerifyEmail(new VerifyEmailRequest("foo", "bar", "baz"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
