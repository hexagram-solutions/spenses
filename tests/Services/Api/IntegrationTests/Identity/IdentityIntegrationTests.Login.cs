using System.Net;
using Bogus;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Login_with_valid_credentials_yields_success()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "donkey.teeth@boisestate.edu",
            Password = new Faker().Internet.Password(),
            DisplayName = "DONKEY TEETH"
        };

        await fixture.Register(registerRequest);

        var (userId, code, _) = fixture.GetVerificationParametersForEmail(registerRequest.Email);

        await _identityApi.VerifyEmail(new VerifyEmailRequest(userId, code));

        var response = await fixture.Login(new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content!.Succeeded.Should().BeTrue();

        await fixture.DeleteUser(registerRequest.Email);
    }

    [Fact]
    public async Task Login_with_invalid_credentials_yields_unauthorized()
    {
        var response = await _identityApi.Login(new LoginRequest
        {
            Email = fixture.VerifiedUser.Email,
            Password = "foo"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var errorContent = await response.Error!.GetContentAsAsync<LoginResult>();

        errorContent!.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task Login_with_unverified_email_yields_unauthorized()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "donkey.teeth@boisestate.edu",
            Password = new Faker().Internet.Password(),
            DisplayName = "DONKEY TEETH"
        };

        await fixture.Register(registerRequest);

        var response = await fixture.Login(new LoginRequest
        {
            Email = registerRequest.Email, Password = registerRequest.Password
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var errorContent = await response.Error!.GetContentAsAsync<LoginResult>();

        errorContent!.Succeeded.Should().BeFalse();
        errorContent.IsNotAllowed.Should().BeTrue();

        await fixture.DeleteUser(registerRequest.Email);
    }
}
