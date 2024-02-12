using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Me;

public partial class MeIntegrationTests : IdentityIntegrationTestBase
{
    private readonly IMeApi _meApi;

    public MeIntegrationTests(IdentityWebApplicationFixture fixture) : base(fixture)
    {
        _meApi = CreateApiClient<IMeApi>();
    }
}
