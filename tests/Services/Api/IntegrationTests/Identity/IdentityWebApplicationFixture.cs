using System.Text.RegularExpressions;
using System.Web;
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
    private readonly HttpClient _authenticatedHttpClient;

    public IdentityWebApplicationFixture()
    {
        WebApplicationFactory = new IdentityWebApplicationFactory<TStartup>();

        _authenticatedHttpClient = WebApplicationFactory.CreateClient();

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
        var email = "grunky.peep@georgiasouthern.edu";
        var password = "Password1!"; // todo: get from config

        //var registerResponse = await Register(new RegisterRequest
        //{
        //    Email = email,
        //    Password = password,
        //    DisplayName = "Grunky Peep"
        //}, true);

        //VerifiedUser = registerResponse.Content!;

        var resp = await Login(new LoginRequest { Email = email, Password = password });

        VerifiedUser = (await RestService.For<IMeApi>(CreateClient()).GetMe()).Content!;
    }

    public async Task DisposeAsync()
    {
        //await DeleteUser("grunky.peep@georgiasouthern.edu");

        await WebApplicationFactory.DisposeAsync();
    }

    public HttpClient CreateClient()
    {
        return _authenticatedHttpClient;
    }

    public async Task<IApiResponse<CurrentUser>> Register(RegisterRequest request, bool verify = false)
    {
        var identityApi = RestService.For<IIdentityApi>(CreateClient());

        var response = await identityApi.Register(request);

        if (!verify)
            return response;

        var currentUser = response.Content!;

        await VerifyUser(currentUser.Email);

        currentUser.EmailVerified = true;

        return response;
    }

    public Task<IApiResponse> VerifyUser(string email)
    {
        var (userId, code, _) = GetVerificationParametersForEmail(email);

        var identityApi = RestService.For<IIdentityApi>(CreateClient());

        return identityApi.VerifyEmail(new VerifyEmailRequest(userId, code));
    }

    public Task<IApiResponse<LoginResult>> Login(LoginRequest loginRequest)
    {
        var identityApi = RestService.For<IIdentityApi>(CreateClient());

        return identityApi.Login(loginRequest);
    }

    public Task<IApiResponse> Logout()
    {
        var identityApi = RestService.For<IIdentityApi>(CreateClient());

        return identityApi.Logout();
    }

    public async Task DeleteUser(string email)
    {
        var userManager = WebApplicationFactory.Services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(email) is not { } user)
            return;

        await userManager.DeleteAsync(user);
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
