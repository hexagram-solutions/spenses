using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Bogus;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Respawn;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Application.Services.Invitations;
using Spenses.Client.Http;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Common;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;
using Spenses.Tools.Setup;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.IntegrationTests;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public abstract class IdentityIntegrationTestBase : IAsyncLifetime
{
    private readonly IdentityWebApplicationFixture _webApplicationFixture;
    private readonly IServiceProvider _services;
    private readonly HttpClient _authenticatedHttpClient;

    private bool _isAuthenticated;

    protected IdentityIntegrationTestBase(IdentityWebApplicationFixture fixture)
    {
        _webApplicationFixture = fixture;
        _services = fixture.WebApplicationFactory.Services;

        _authenticatedHttpClient = _webApplicationFixture.CreateClient();
    }

    public CurrentUser VerifiedUser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await ResetDatabase();

        VerifiedUser = (await RestService.For<IMeApi>(CreateAuthenticatedClient()).GetMe()).Content!;

        await LoginAsTestUser();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task ResetDatabase()
    {
        await using var scope = _services.CreateAsyncScope();

        var services = scope.ServiceProvider;

        var userContextProvider = services.GetRequiredService<UserContextProvider>();
        userContextProvider.SetContext(services.GetRequiredService<SystemUserContext>());

        var db = services.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();

        var connectionString = db.Database.GetConnectionString()!;

        var respawner = await Respawner.CreateAsync(connectionString, new RespawnerOptions
        {
            CheckTemporalTables = true
        });

        await respawner.ResetAsync(connectionString);

        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedDatabase();

        userContextProvider.SetContext(services.GetRequiredService<IUserContext>());
    }

    public async Task ExecuteDbContextAction(Func<ApplicationDbContext, Task> action)
    {
        await using var scope = _services.CreateAsyncScope();

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
        return _webApplicationFixture.CreateClient();
    }

    public async Task LoginAsTestUser()
    {
        var config = _services.GetRequiredService<IConfiguration>();

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
        var userManager = _services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(email) is not { } user)
            return;

        await userManager.DeleteAsync(user);
    }

    public CapturedEmailMessage GetLastMessageForEmail(string email)
    {
        var emailClient = _services.GetRequiredService<CapturingEmailClient>();

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

        var tokenProvider = _services.GetRequiredService<InvitationTokenProvider>();

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

public class IntegrationTestClass1 : IdentityIntegrationTestBase
{
    private readonly IIdentityApi _identityApi;

    public IntegrationTestClass1(IdentityWebApplicationFixture fixture)
        : base(fixture)
    {
        _identityApi = RestService.For<IIdentityApi>(CreateClient());
    }

    [Fact]
    public async Task Login_with_valid_credentials_yields_success()
    {
        var faker = new Faker();

        var registerRequest = new RegisterRequest
        {
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            DisplayName = "DONKEY TEETH"
        };

        var resp = await Register(registerRequest);

        var (userId, code, _) = GetVerificationParametersForEmail(registerRequest.Email);

        await _identityApi.VerifyEmail(new VerifyEmailRequest(userId, code));

        var response = await Login(new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        response.Content!.Succeeded.Should().BeTrue();
    }
}

public class HomeIntegrationTests2 : IdentityIntegrationTestBase
{
    private readonly IHomesApi _homesApi;

    public HomeIntegrationTests2(IdentityWebApplicationFixture fixture) : base(fixture)
    {
        _homesApi = RestService.For<IHomesApi>(CreateAuthenticatedClient());
    }

    [Fact]
    public async Task Get_homes()
    {
        var response = await _homesApi.GetHomes();

        response.Content.Should().NotBeEmpty();
    }
}
