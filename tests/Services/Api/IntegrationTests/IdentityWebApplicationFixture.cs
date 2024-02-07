using System.Text.RegularExpressions;
using System.Web;
using FluentAssertions.Extensions;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Application.Services.Invitations;
using Spenses.Client.Http;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Common;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests;

public class IdentityWebApplicationFixture<TStartup> : IAsyncLifetime
    where TStartup : class
{
    private readonly HttpClient _authenticatedHttpClient;

    private bool _isAuthenticated;

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
        VerifiedUser = (await RestService.For<IMeApi>(CreateAuthenticatedClient()).GetMe()).Content!;
    }

    public async Task DisposeAsync()
    {
        await WebApplicationFactory.DisposeAsync();
    }

    public async Task ExecuteDbContextAction(Func<ApplicationDbContext, Task> action)
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await action(db);
    }

    public HttpClient CreateAuthenticatedClient()
    {
        if (_isAuthenticated)
            return _authenticatedHttpClient;

        LoginAsTestUser().Wait();

        return _authenticatedHttpClient;
    }

    public HttpClient CreateClient()
    {
        return WebApplicationFactory.CreateClient();
    }

    public async Task LoginAsTestUser()
    {
        var config = WebApplicationFactory.Services.GetRequiredService<IConfiguration>();

        var email = config.Require(ConfigConstants.SpensesTestIntegrationTestUserEmail);
        var password = config.Require(ConfigConstants.SpensesTestDefaultUserPassword);

        await Login(new LoginRequest { Email = email, Password = password });
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

        var identityApi = RestService.For<IIdentityApi>(CreateAuthenticatedClient());

        return identityApi.VerifyEmail(new VerifyEmailRequest(userId, code));
    }

    public async Task<IApiResponse<LoginResult>> Login(LoginRequest loginRequest)
    {
        var identityApi = RestService.For<IIdentityApi>(_authenticatedHttpClient);

        var response = await identityApi.Login(loginRequest);

        _isAuthenticated = response.IsSuccessStatusCode;

        return response;
    }

    public async Task<IApiResponse> Logout()
    {
        var identityApi = RestService.For<IIdentityApi>(CreateAuthenticatedClient());

        var response = await identityApi.Logout();

        _isAuthenticated = !response.IsSuccessStatusCode;

        return response;
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

    public Guid GetInvitationIdForEmail(string email)
    {
        var token = GetInvitationTokenForEmail(email);

        var tokenProvider = WebApplicationFactory.Services.GetRequiredService<InvitationTokenProvider>();

        return tokenProvider.UnprotectInvitationData(token).InvitationId;
    }

    public string GetInvitationTokenForEmail(string email)
    {
        var message = GetLastMessageForEmail(email);

        var invitationAcceptUri = GetLinkFromEmailMessage(message);

        var parameters = HttpUtility.ParseQueryString(invitationAcceptUri.Query);

        return parameters["invitationToken"]!;
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
