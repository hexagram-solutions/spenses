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

    [Fact]
    public async Task Put_home_updates_home()
    {
        var home = (await _homes.GetHomes()).First();

        var properties = new HomeProperties { Name = "sut", Description = "baz" };

        var updatedHome = await _homes.PutHome(home.Id, properties);
        updatedHome.Should().BeEquivalentTo(properties);

        var retrievedHome = await _homes.GetHome(updatedHome.Id);
        retrievedHome.Should().BeEquivalentTo(updatedHome);
    }

    [Fact]
    public async Task Post_home_member_creates_member()
    {
        var home = (await _homes.GetHomes()).First(x => x.Members.Any());

        var properties = new MemberProperties { Name = "Bob", AnnualTakeHomeIncome = 80_000.00m };

        var createdMember = await _homes.PostHomeMember(home.Id, properties);
        createdMember.Should().BeEquivalentTo(properties);

        var retrievedHome = await _homes.GetHome(home.Id);
        retrievedHome.Members.Should().ContainEquivalentOf(createdMember);
    }

    [Fact]
    public async Task Put_home_member_updates_member()
    {
        var home = (await _homes.GetHomes()).First(x => x.Members.Any());

        var member = home.Members.First();

        var properties = new MemberProperties { Name = "Grunky Peep", AnnualTakeHomeIncome = 1m };

        var updatedMember = await _homes.PutHomeMember(home.Id, member.Id, properties);
        updatedMember.Should().BeEquivalentTo(properties);

        var retrievedMember = await _homes.GetHomeMember(home.Id, member.Id);
        retrievedMember.Should().BeEquivalentTo(updatedMember);
    }
}
