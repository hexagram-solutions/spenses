using System.Net;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Resend_verification_email_sends_email_to_user()
    {
        var email = VerifiedUser.Email;

        var response = await _identityApi.ResendVerificationEmail(new ResendVerificationEmailRequest(email));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var message = GetLastMessageForEmail(email);

        message.Subject.Should().Contain("Verify");
    }

    [Fact]
    public async Task Resend_verification_email_with_invalid_email_yields_bad_request()
    {
        var response = await _identityApi.ResendVerificationEmail(new ResendVerificationEmailRequest("foo"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Resend_verification_email_for_non_existent_user_fails_silently()
    {
        var response = await _identityApi.ResendVerificationEmail(
            new ResendVerificationEmailRequest("donkey.teeth@boisestate.edu"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
