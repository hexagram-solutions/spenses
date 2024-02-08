using Bogus;
using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Members;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class MembersIntegrationTests(IdentityWebApplicationFixture<Program> fixture) : IAsyncLifetime
{
    private readonly Faker _faker = new();

    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());
    private readonly IMembersApi _members = RestService.For<IMembersApi>(fixture.CreateAuthenticatedClient());

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await fixture.LoginAsTestUser();
    }
}
