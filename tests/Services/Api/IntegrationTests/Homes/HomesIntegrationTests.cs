using Refit;
using Spenses.Application.Models;
using Spenses.Client;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(WebApplicationCollection.CollectionName)]
public class HomesIntegrationTests
{
    private readonly IHomesApi _homes;

    public HomesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
    }

    [Fact]
    public async Task Post_creates_home()
    {
        var properties = new HomeProperties { Name = "foo", Description = "bar" };

        var createdHome = await _homes.PostHome(properties);

        createdHome.Should().BeEquivalentTo(properties);

        var retrievedHome = await _homes.GetHome(createdHome.Id);

        retrievedHome.Should().BeEquivalentTo(createdHome);

        var homes = await _homes.GetHomes();

        homes.Should().ContainEquivalentOf(retrievedHome);
    }
}
