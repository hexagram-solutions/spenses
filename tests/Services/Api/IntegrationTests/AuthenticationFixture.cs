using System.Text.RegularExpressions;
using System.Web;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Application.Services.Invitations;
using Spenses.Client.Http;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Common;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public class AuthenticationFixture
{
    private readonly IdentityWebApplicationFixture _appFixture;
    private readonly HttpClient _authenticatedHttpClient;

    private bool _isAuthenticated;

    public AuthenticationFixture(IdentityWebApplicationFixture appFixture)
    {
        _appFixture = appFixture;
        Services = appFixture.WebApplicationFactory.Services;

        _authenticatedHttpClient = _appFixture.WebApplicationFactory.CreateClient();
    }

    public IServiceProvider Services { get; }

    public CurrentUser VerifiedUser { get; private set; } = null!;

    public HttpClient CreateAuthenticatedClient()
    {
        if (_isAuthenticated)
            return _authenticatedHttpClient;

        LoginAsTestUser().Wait();

        return _authenticatedHttpClient;
    }

    public HttpClient CreateClient()
    {
        return _appFixture.WebApplicationFactory.CreateClient();
    }

    public TClient CreateApiClient<TClient>(bool authenticated = true)
    {
        var settings = new RefitSettings { CollectionFormat = CollectionFormat.Multi };

        return authenticated
            ? RestService.For<TClient>(CreateAuthenticatedClient(), settings)
            : RestService.For<TClient>(CreateClient(), settings);
    }

    public async Task LoginAsTestUser()
    {
        var config = Services.GetRequiredService<IConfiguration>();

        var email = config.Require(ConfigConstants.SpensesTestIntegrationTestUserEmail);
        var password = config.Require(ConfigConstants.SpensesTestDefaultUserPassword);

        await Login(new LoginRequest { Email = email, Password = password });
    }

    public async Task<IApiResponse<CurrentUser>> Register(RegisterRequest request, bool verify = false)
    {
        var identityApi = CreateApiClient<IIdentityApi>();

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

        var identityApi = CreateApiClient<IIdentityApi>();

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
        var identityApi = CreateApiClient<IIdentityApi>();

        var response = await identityApi.Logout();

        _isAuthenticated = !response.IsSuccessStatusCode;

        return response;
    }

    public async Task DeleteUser(string email)
    {
        var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(email) is not { } user)
            return;

        await userManager.DeleteAsync(user);
    }

    public CapturedEmailMessage GetLastMessageForEmail(string email)
    {
        var emailClient = Services.GetRequiredService<CapturingEmailClient>();

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

        var tokenProvider = Services.GetRequiredService<InvitationTokenProvider>();

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
