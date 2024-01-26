using System.Net;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Delete_home_member_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var homeNotFoundResult = await _members.DeleteMember(Guid.NewGuid(), member.Id);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberNotFoundResult = await _members.DeleteMember(home.Id, Guid.NewGuid());

        memberNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_member_with_no_associations_results_in_hard_deletion()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var newMember = (await _members.PostMember(homeId, new CreateMemberProperties
        {
            Name = "Grunky Peep",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = "grunky.peep@georgiasouthern.edu"
        })).Content!;

        var deleteMemberResponse = await _members.DeleteMember(homeId, newMember.Id);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        deleteMemberResponse.Content!.Model.Should().BeEquivalentTo(newMember);

        var memberResponse = await _members.GetMember(homeId, newMember.Id);
        memberResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var members = (await _members.GetMembers(homeId)).Content!;
        members.Should().NotContain(x => x.Id == newMember.Id);

        var homeMembers = (await _homes.GetHome(homeId)).Content!.Members;
        homeMembers.Should().NotContain(x => x.Id == newMember.Id);
    }

    [Fact]
    public async Task Delete_member_with_associations_results_in_deactivation()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var deleteMemberResponse = await _members.DeleteMember(home.Id, member.Id);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deleteMemberResult = deleteMemberResponse.Content!;

        deleteMemberResult.Model.Should().BeEquivalentTo(member, opts =>
            opts.Excluding(m => m.Status));

        deleteMemberResult.Model.Status.Should().Be(MemberStatus.Inactive);

        deleteMemberResult.Type.Should().Be(DeletionType.Deactivated);

        var deactivatedMember = (await _members.GetMember(home.Id, member.Id)).Content!;
        deactivatedMember.Status.Should().Be(MemberStatus.Inactive);

        var members = (await _members.GetMembers(home.Id)).Content!;
        members.Should().Contain(x => x.Id == deactivatedMember.Id && x.Status == MemberStatus.Inactive);

        var homeMembers = (await _homes.GetHome(home.Id)).Content!.Members;
        homeMembers.Should().Contain(x => x.Id == deactivatedMember.Id && x.Status == MemberStatus.Inactive);

        await _members.ActivateMember(home.Id, member.Id);
    }

    [Fact]
    public async Task Delete_member_with_no_associations_and_pending_invitation_deletes_invitation()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Delete_member_with_associations_and_pending_invitation_cancels_invitation()
    {
        throw new NotImplementedException();
    }
}
