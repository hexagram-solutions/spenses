using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Members;

[Collection(WebApplicationCollection.CollectionName)]
public partial class MembersIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IMembersApi _members;

    public MembersIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
        _members = RestService.For<IMembersApi>(fixture.WebApplicationFactory.CreateClient());
    }
}
