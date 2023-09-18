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
        // todo: json settings, should be able to just DI this, no? but maybe not depending on when server gets started up
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
        // TODO: Use seed data
        var properties = new HomeProperties { Name = "foo", Description = "bar" };

        var createdHome = await _homes.PostHome(properties);

        properties = new HomeProperties { Name = "sut", Description = "baz" };

        var updatedHome = await _homes.PutHome(createdHome.Id, properties);
        updatedHome.Should().BeEquivalentTo(properties);

        var retrievedHome = await _homes.GetHome(updatedHome.Id);
        retrievedHome.Should().BeEquivalentTo(updatedHome);
    }

    [Fact]
    public async Task Post_home_member_creates_member()
    {
        // TODO: Use seed data
        var createdHome = await _homes.PostHome(new HomeProperties { Name = "foo", Description = "bar" });

        var properties = new MemberProperties { Name = "Bob", AnnualTakeHomeIncome = 80_000.00m };

        var createdMember = await _homes.PostHomeMember(createdHome.Id, properties);
        createdMember.Should().BeEquivalentTo(properties);

        var retrievedHome = await _homes.GetHome(createdHome.Id);
        retrievedHome.Members.Should().ContainEquivalentOf(createdMember);
    }

    [Fact]
    public async Task Put_home_member_updates_member()
    {
        // TODO: Use seed data
        var createdHome = await _homes.PostHome(new HomeProperties { Name = "foo", Description = "bar" });

        var properties = new MemberProperties { Name = "Bob", AnnualTakeHomeIncome = 80_000.00m };

        var createdMember = await _homes.PostHomeMember(createdHome.Id, properties);

        properties = new MemberProperties { Name = "Alice", AnnualTakeHomeIncome = 1m };

        var updatedMember = await _homes.PutHomeMember(createdHome.Id, createdMember.Id, properties);
        updatedMember.Should().BeEquivalentTo(properties);

        var retrievedMember = await _homes.GetHomeMember(createdHome.Id, updatedMember.Id);
        retrievedMember.Should().BeEquivalentTo(updatedMember);
    }
}
