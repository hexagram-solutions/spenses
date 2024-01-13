using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Me;

[Collection(WebApplicationCollection.CollectionName)]
public partial class MeIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IMeApi _meApi = RestService.For<IMeApi>(fixture.WebApplicationFactory.CreateClient());
}
