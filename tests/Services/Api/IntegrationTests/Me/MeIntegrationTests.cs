using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Me;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class MeIntegrationTests(IdentityWebApplicationFixture<Program> fixture)
{
    private readonly IMeApi _meApi = RestService.For<IMeApi>(fixture.CreateClient());
}
