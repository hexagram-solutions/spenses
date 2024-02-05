using System.Net;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Activate_inactive_member_activates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var deleteMemberResponse = await _members.DeleteMember(home.Id, member.Id);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var activateMemberResponse = await _members.ActivateMember(home.Id, member.Id);

        activateMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        activateMemberResponse.Content.Should().BeEquivalentTo(member, opts => opts.Excluding(m => m.AvatarUrl));
    }

    [Fact]
    public async Task Activate_currently_active_member_yields_success()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var activateMemberResponse = await _members.ActivateMember(home.Id, member.Id);

        activateMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        activateMemberResponse.Content.Should().BeEquivalentTo(member, opts => opts.Excluding(m => m.AvatarUrl));
    }

    [Fact]
    public async Task Activate_home_member_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var homeNotFoundResult = await _members.ActivateMember(Guid.NewGuid(), member.Id);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberNotFoundResult = await _members.ActivateMember(home.Id, Guid.NewGuid());

        memberNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
