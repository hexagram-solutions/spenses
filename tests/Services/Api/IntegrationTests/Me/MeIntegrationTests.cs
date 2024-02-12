using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Me;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class MeIntegrationTests(IdentityWebApplicationFixture fixture) : IAsyncLifetime
{
    private readonly IMeApi _meApi = RestService.For<IMeApi>(fixture.CreateAuthenticatedClient());

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.LoginAsTestUser();
    }
}
