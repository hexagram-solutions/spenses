using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests
{
    [Fact]
    public async Task Verify_email_using_parameters_from_email_sets_email_as_verified()
    {
        // Email will have already been sent during test setup
        var (userId, code) = GetVerificationParameters();

        var response = await _identityApi.VerifyEmail(new VerifyEmailRequest(userId, code));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private (string userId, string code) GetVerificationParameters()
    {
        var emailClient = fixture.WebApplicationFactory.Services.GetRequiredService<CapturingEmailClient>();

        var message = emailClient.EmailMessages
            .Single(e => e.RecipientAddress == fixture.RegisteredUser.Email);

        var regex = new Regex("<a [^>]*href=(?:'(?<href>.*?)')|(?:\"(?<href>.*?)\")", RegexOptions.IgnoreCase);

        var verificationAnchorValue = regex.Matches(message.HtmlMessage)
            .Select(m => m.Groups["href"].Value)
            .Single();

        var confirmationUri = new Uri(verificationAnchorValue);

        var parameters = HttpUtility.ParseQueryString(confirmationUri.Query);

        return (parameters["userId"]!, parameters["code"]!);
    }

    [Fact]
    public async Task Verify_email_with_invalid_parameters_yields_unauthorized()
    {
        var response = await _identityApi.VerifyEmail(new VerifyEmailRequest("foo", "bar"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
