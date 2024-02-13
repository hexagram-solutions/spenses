namespace Spenses.Api.IntegrationTests;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public abstract class IdentityIntegrationTestBase(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    : IAsyncLifetime, IClassFixture<DatabaseFixture>, IClassFixture<AuthenticationFixture>
{
    public virtual async Task InitializeAsync()
    {
        await DatabaseFixture.ResetDatabase();

        await AuthFixture.LoginAsTestUser();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected AuthenticationFixture AuthFixture => authFixture;

    protected DatabaseFixture DatabaseFixture => databaseFixture;

    protected TClient CreateApiClient<TClient>(bool authenticated = true)
    {
        return AuthFixture.CreateApiClient<TClient>(authenticated);
    }
}
