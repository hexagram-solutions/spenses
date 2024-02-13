using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    : IdentityIntegrationTestBase(databaseFixture, authFixture)
{
    private IMeApi _meApi = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _meApi = CreateApiClient<IMeApi>();
    }
}
