using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Payments;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class PaymentsIntegrationTests(IdentityWebApplicationFixture<Program> fixture) : IAsyncLifetime
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());

    private readonly IPaymentsApi _payments =
        RestService.For<IPaymentsApi>(fixture.CreateAuthenticatedClient());

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.LoginAsTestUser();
    }
}
