using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(WebApplicationCollection.CollectionName)]
public partial class HomesIntegrationTests
{
    private readonly WebApplicationFixture<Program> _fixture;

    private readonly IHomesApi _homes;

    public HomesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _fixture = fixture;

        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
    }
}
