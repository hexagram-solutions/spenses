using System.Net;
using Bogus;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Forgot_password_sends_reset_email_for_verified_user()
    {
        var response = await _identityApi.ForgotPassword(
            new ForgotPasswordRequest { Email = fixture.VerifiedUser.Email });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var (email, _) = fixture.GetPasswordResetParametersForEmail(fixture.VerifiedUser.Email);

        email.Should().Be(fixture.VerifiedUser.Email);
    }

    [Fact]
    public async Task Forgot_password_fails_silently_for_non_verified_user()
    {
        var registerRequest = new RegisterRequest
        {
            Name = "Quatro Quatro",
            Email = "quatro.quatro@sjsu.edu",
            Password = new Faker().Internet.Password()
        };

        await _identityApi.Register(registerRequest);

        var response = await _identityApi.ForgotPassword(new ForgotPasswordRequest { Email = registerRequest.Email });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await fixture.DeleteUser(registerRequest.Email);
    }

    [Fact]
    public async Task Forgot_password_fails_silently_for_non_existent_user()
    {
        var response = await _identityApi.ForgotPassword(
             new ForgotPasswordRequest { Email = "quatro.quatro@sjsu.edu" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
