using System.Text.RegularExpressions;
using System.Web;
using Bogus;
using FluentAssertions.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Client.Http;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests.Identity;

public class IdentityWebApplicationFixture<TStartup> : IAsyncLifetime
    where TStartup : class
{
    public IdentityWebApplicationFixture()
    {
        WebApplicationFactory = new IdentityWebApplicationFactory<TStartup>();

        // Since this is static, it will set equivalency rules for the entire test run. This could be configured
        // anywhere, but this class is "global" enough for such configuration.
        AssertionOptions.AssertEquivalencyUsing(opts =>
            opts.Using<DateTime>(ctx =>
                ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100.Milliseconds())).WhenTypeIs<DateTime>());
    }

    public IdentityWebApplicationFactory<TStartup> WebApplicationFactory { get; }

    public CurrentUser VerifiedUser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var identityApi = RestService.For<IIdentityApi>(WebApplicationFactory.CreateClient());

        var registerRequest = new RegisterRequest
        {
            Email = "grunky.peep@georgiasouthern.edu",
            Password = new Faker().Internet.Password(),
            Name = "Grunky Peep"
        };

        var response = await identityApi.Register(registerRequest);

        VerifiedUser = response.Content!;

        var (userId, code, _) = GetVerificationParametersForEmail(registerRequest.Email);

        await identityApi.VerifyEmail(new VerifyEmailRequest(userId, code));

        VerifiedUser.EmailVerified = true;
    }

    public async Task DisposeAsync()
    {
        await DeleteUser("grunky.peep@georgiasouthern.edu");

        var emailClient = WebApplicationFactory.Services.GetRequiredService<CapturingEmailClient>();
        emailClient.EmailMessages.Clear();

        await WebApplicationFactory.DisposeAsync();
    }

    public async Task DeleteUser(string email)
    {
        var userManager = WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);

        await userManager.DeleteAsync(user!);
    }

    public CapturedEmailMessage GetLastMessageForEmail(string email)
    {
        var emailClient = WebApplicationFactory.Services.GetRequiredService<CapturingEmailClient>();

        return emailClient.EmailMessages
            .Last(e => e.RecipientAddress == email);
    }

    public (string userId, string code, string? newEmail) GetVerificationParametersForEmail(string email)
    {
        var message = GetLastMessageForEmail(email);

        var confirmationUri = GetLinkFromEmailMessage(message);

        var parameters = HttpUtility.ParseQueryString(confirmationUri.Query);

        return (parameters["userId"]!, parameters["code"]!, parameters["newEmail"]);
    }

    public (string email, string code) GetPasswordResetParametersForEmail(string email)
    {
        var message = GetLastMessageForEmail(email);

        var confirmationUri = GetLinkFromEmailMessage(message);

        var parameters = HttpUtility.ParseQueryString(confirmationUri.Query);

        return (parameters["email"]!, parameters["code"]!);
    }

    private static Uri GetLinkFromEmailMessage(CapturedEmailMessage message)
    {
        var regex = new Regex("<a [^>]*href=(?:'(?<href>.*?)')|(?:\"(?<href>.*?)\")", RegexOptions.IgnoreCase);

        var verificationAnchorValue = regex.Matches(message.HtmlMessage)
            .Select(m => m.Groups["href"].Value)
            .Single();

        return new Uri(verificationAnchorValue);
    }
}
