using Refit;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.IntegrationTests;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public abstract class IdentityIntegrationTestBase :
    IAsyncLifetime,
    IClassFixture<DatabaseFixture>,
    IClassFixture<AuthenticationFixture>
{
    private readonly DatabaseFixture _databaseFixture;
    private readonly AuthenticationFixture _authFixture;

    protected IdentityIntegrationTestBase(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    {
        _databaseFixture = databaseFixture;
        _authFixture = authFixture;
    }

    public CurrentUser VerifiedUser { get; private set; } = null!;

    // todo: stinky
    public IServiceProvider Services => _authFixture.Services; 

    public virtual async Task InitializeAsync()
    {
        await _databaseFixture.ResetDatabase();

        await _authFixture.LoginAsTestUser();

        VerifiedUser = _authFixture.VerifiedUser;
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task ExecuteDbContextAction(Func<ApplicationDbContext, Task> action)
    {
        return _databaseFixture.ExecuteDbContextAction(action);
    }

    public HttpClient CreateAuthenticatedClient()
    {
        return _authFixture.CreateAuthenticatedClient();
    }

    public HttpClient CreateClient()
    {
        return _authFixture.CreateClient();
    }

    protected TClient CreateApiClient<TClient>(bool authenticated = true)
    {
        return _authFixture.CreateApiClient<TClient>(authenticated);
    }

    public Task LoginAsTestUser()
    {
        return _authFixture.LoginAsTestUser();
    }

    public Task<IApiResponse<CurrentUser>> Register(RegisterRequest request, bool verify = false)
    {
        return _authFixture.Register(request, verify);
    }

    public Task<IApiResponse> VerifyUser(string email)
    {
        return _authFixture.VerifyUser(email);
    }

    public Task<IApiResponse<LoginResult>> Login(LoginRequest loginRequest)
    {
        return _authFixture.Login(loginRequest);
    }

    public Task<IApiResponse> Logout()
    {
        return _authFixture.Logout();
    }

    public Task DeleteUser(string email)
    {
        return _authFixture.DeleteUser(email);
    }

    public CapturedEmailMessage GetLastMessageForEmail(string email)
    {
        return _authFixture.GetLastMessageForEmail(email);
    }

    public (string userId, string code, string? newEmail) GetVerificationParametersForEmail(string email)
    {
        return _authFixture.GetVerificationParametersForEmail(email);
    }

    public (string email, string code) GetPasswordResetParametersForEmail(string email)
    {
        return _authFixture.GetPasswordResetParametersForEmail(email);
    }

    public Guid GetInvitationIdForEmail(string email)
    {
        return _authFixture.GetInvitationIdForEmail(email);
    }

    public string GetInvitationTokenForEmail(string email)
    {
        return _authFixture.GetInvitationTokenForEmail(email);
    }
}
