using System.Net;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Forgot_password_sends_reset_email_for_verified_user()
    {
        var verifiedEmail = AuthFixture.CurrentUser.Email;

        var response = await _identityApi.ForgotPassword(
            new ForgotPasswordRequest { Email = verifiedEmail });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var (email, _) = AuthFixture.GetPasswordResetParametersForEmail(verifiedEmail);

        email.Should().Be(verifiedEmail);
    }

    [Fact]
    public async Task Forgot_password_fails_silently_for_non_verified_user()
    {
        var registerRequest = new RegisterRequest
        {
            DisplayName = "Quatro Quatro",
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password()
        };

        await AuthFixture.Register(registerRequest);

        var response = await _identityApi.ForgotPassword(new ForgotPasswordRequest { Email = registerRequest.Email });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Forgot_password_fails_silently_for_non_existent_user()
    {
        var response = await _identityApi.ForgotPassword(
             new ForgotPasswordRequest { Email = "quatro.quatro@sjsu.edu" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
