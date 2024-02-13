using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests : IdentityIntegrationTestBase
{
    private readonly IMeApi _meApi;

    public MeIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
        : base(databaseFixture, authFixture)
    {
        _meApi = CreateApiClient<IMeApi>();
    }
}
