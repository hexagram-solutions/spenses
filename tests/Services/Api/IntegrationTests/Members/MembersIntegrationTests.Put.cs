using System.Net;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Put_home_member_updates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var properties = new MemberProperties
        {
            Name = "Grunky Peep",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = "grunky.peep@georgiasouthern.edu"
        };

        var updatedMemberResponse = await _members.PutMember(home.Id, member.Id, properties);

        updatedMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedMember = updatedMemberResponse.Content!;

        updatedMember.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        updatedMember.IsActive.Should().BeTrue();

        var fetchedMember = (await _members.GetMember(home.Id, member.Id)).Content;
        fetchedMember.Should().BeEquivalentTo(updatedMember);
    }

    [Fact]
    public async Task Put_invalid_member_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var memberId = home.Members.First();

        var result = await _members.PutMember(home.Id, memberId.Id, new MemberProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_home_member_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var homeNotFoundResult = await _members.PutMember(Guid.NewGuid(), member.Id, member);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberNotFoundResult = await _members.PutMember(home.Id, Guid.NewGuid(), member);

        memberNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_home_inactive_home_does_not_change_is_active_property()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var memberId = home.Members.First().Id;

        var member = (await _members.DeleteMember(home.Id, memberId)).Content!.Model;

        var updatedMemberResponse = await _members.PutMember(home.Id, memberId, member);

        updatedMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedMember = updatedMemberResponse.Content!;

        updatedMember.Should().BeEquivalentTo(member);

        await _members.ActivateMember(home.Id, member.Id);
    }
}
