using Refit;
using Spenses.Application.Models;
using Spenses.Client.Http;

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

        await _homes.DeleteHome(createdHome.Id);
    }

    [Fact]
    public async Task Put_home_updates_home()
    {
        var home = (await _homes.GetHomes()).First();

        var properties = new HomeProperties { Name = "sut", Description = "baz" };

        var updatedHome = await _homes.PutHome(home.Id, properties);
        updatedHome.Should().BeEquivalentTo(properties);

        var retrievedHome = await _homes.GetHome(updatedHome.Id);
        retrievedHome.Should().BeEquivalentTo(updatedHome, opts => opts.Excluding(x => x.Members));
    }
}
