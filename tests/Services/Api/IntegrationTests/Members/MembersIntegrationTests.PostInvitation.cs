using System.Net;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Post_invitation_sends_invitation_to_existing_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new CreateMemberProperties
        {
            Name = "Quatro Quatro",
            DefaultSplitPercentage = 0.0m,
        };

        var email = "quatro.quatro@sjsu.edu";

        var createdMemberResponse = await _members.PostMember(home.Id, properties);

        createdMemberResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdMember = createdMemberResponse.Content!;

        var invitationResponse = await _members.PostMemberInvitation(
            home.Id, createdMember.Id, new InvitationProperties { Email = email });

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var invitation = invitationResponse.Content!;

        invitation.Member.Id.Should().Be(createdMember.Id);
        invitation.Email.Should().Be(email);
        invitation.Status.Should().Be(InvitationStatus.Pending);

        var invitationMessage = GetLastMessageForEmail(email);

        invitationMessage.RecipientAddress.Should().Be(email);
        invitationMessage.Subject.Should().Contain(home.Name);
        invitationMessage.PlainTextMessage.Should().Contain("?invitationToken=");

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Post_invitation_for_member_cancels_pending_invitations()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new CreateMemberProperties
        {
            Name = "Quatro Quatro",
            DefaultSplitPercentage = 0.0m,
        };

        var email = "quatro.quatro@sjsu.edu";

        var createdMember = (await _members.PostMember(home.Id, properties)).Content!;

        var invitationResponse1 = await _members.PostMemberInvitation(
            home.Id, createdMember.Id, new InvitationProperties { Email = email });

        var invitationResponse2 = await _members.PostMemberInvitation(
            home.Id, createdMember.Id, new InvitationProperties { Email = email });

        var memberInvitations = (await _members.GetMemberInvitations(home.Id, createdMember.Id)).Content!;

        memberInvitations.Should().AllSatisfy(i => i.Member.Id.Should().Be(createdMember.Id));

        memberInvitations.Should().ContainSingle(i =>
            i.Id == invitationResponse1.Content!.Id && i.Status == InvitationStatus.Cancelled);

        memberInvitations.Should().ContainSingle(i =>
            i.Id == invitationResponse2.Content!.Id && i.Status == InvitationStatus.Pending);

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Post_invitation_for_member_already_associated_with_user_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var member = (await _members.GetMembers(home.Id)).Content!
            .First(m => m.User != null);

        var invitationResponse = await _members.PostMemberInvitation(
            home.Id, member.Id, new InvitationProperties { Email = member.ContactEmail! });

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_invitation_to_member_with_invalid_properties_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var member = (await _members.GetMembers(home.Id)).Content!
            .First(m => m.User != null);

        var invitationResponse = await _members.PostMemberInvitation(
            home.Id, member.Id, new InvitationProperties { Email = "foobar" });

        invitationResponse.Should().HaveValidationErrorFor(x => x.Email);
    }
}
