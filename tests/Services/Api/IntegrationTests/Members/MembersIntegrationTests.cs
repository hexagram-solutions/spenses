using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Members;

[Collection(WebApplicationCollection.CollectionName)]
public partial class MembersIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
    private readonly IMembersApi _members = RestService.For<IMembersApi>(fixture.WebApplicationFactory.CreateClient());
}
