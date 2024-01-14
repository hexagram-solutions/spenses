using System.Net;
using Bogus;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Verify_email_using_parameters_from_email_sets_email_as_verified()
    {
        var registerRequest = new RegisterRequest
        {
            Name = "Quatro Quatro",
            Email = "quatro.quatro@sjsu.edu",
            Password = new Faker().Internet.Password()
        };

        await _identityApi.Register(registerRequest);

        var (userId, code, _) = fixture.GetVerificationParametersForEmail(registerRequest.Email);

        var response = await _identityApi.VerifyEmail(new VerifyEmailRequest(userId, code));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await fixture.DeleteUser(registerRequest.Email);
    }

    [Fact]
    public async Task Verify_email_with_invalid_parameters_yields_bad_request()
    {
        var response = await _identityApi.VerifyEmail(new VerifyEmailRequest("foo", "bar", "baz"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
