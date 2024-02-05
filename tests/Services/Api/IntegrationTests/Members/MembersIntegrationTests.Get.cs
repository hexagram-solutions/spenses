using System.Net;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Get_home_member_yields_success()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var homeMember = home.Members.First();

        var fetchMemberResponse = await _members.GetMember(home.Id, homeMember.Id);

        fetchMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        fetchMemberResponse.Content!.Should().BeEquivalentTo(homeMember, opts => opts.Excluding(m => m.AvatarUrl));
    }

    [Fact]
    public async Task Get_home_member_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var homeNotFoundResult = await _members.GetMember(Guid.NewGuid(), member.Id);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberNotFoundResult = await _members.GetMember(home.Id, Guid.NewGuid());

        memberNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
