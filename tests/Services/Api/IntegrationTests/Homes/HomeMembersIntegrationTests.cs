using Refit;
using Spenses.Application.Models;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(WebApplicationCollection.CollectionName)]
public class HomeMembersIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IHomeMembersApi _homeMembers;

    public HomeMembersIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
        _homeMembers = RestService.For<IHomeMembersApi>(fixture.WebApplicationFactory.CreateClient());
    }

    [Fact]
    public async Task Post_home_member_creates_member()
    {
        var home = (await _homes.GetHomes()).First(x => x.Members.Any());

        var properties = new MemberProperties { Name = "Bob", AnnualTakeHomeIncome = 80_000.00m };

        var createdMember = await _homeMembers.PostHomeMember(home.Id, properties);
        createdMember.Should().BeEquivalentTo(properties);

        var members = await _homeMembers.GetHomeMembers(home.Id);
        members.Should().ContainEquivalentOf(createdMember);

        await _homeMembers.DeleteHomeMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Put_home_member_updates_member()
    {
        var home = (await _homes.GetHomes()).First(x => x.Members.Any());

        var member = home.Members.First();

        var properties = new MemberProperties { Name = "Grunky Peep", AnnualTakeHomeIncome = 1m };

        var updatedMember = await _homeMembers.PutHomeMember(home.Id, member.Id, properties);
        updatedMember.Should().BeEquivalentTo(properties);

        var retrievedMember = await _homeMembers.GetHomeMember(home.Id, member.Id);
        retrievedMember.Should().BeEquivalentTo(updatedMember);
    }
}
