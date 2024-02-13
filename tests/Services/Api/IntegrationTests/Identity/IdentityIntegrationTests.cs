using Bogus;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    : IdentityIntegrationTestBase(databaseFixture, authFixture)
{
    private readonly Faker _faker = new();

    private IIdentityApi _identityApi = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _identityApi = CreateApiClient<IIdentityApi>();
    }
}
