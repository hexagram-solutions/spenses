using System.Net;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Delete_invitation_sets_invitations_to_cancelled()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new CreateMemberProperties
        {
            Name = "Quatro Quatro",
            DefaultSplitPercentage = 0.0m,
        };

        var createdMember = (await _members.PostMember(home.Id, properties)).Content!;

        var email = "quatro.quatro@sjsu.edu";

        var invitationResponse = await _members.PostMemberInvitation(
            home.Id, createdMember.Id, new InvitationProperties { Email = email });

        var invitation = invitationResponse.Content!;

        var cancelInvitationResponse = await _members.CancelMemberInvitations(home.Id, createdMember.Id);
        cancelInvitationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var cancelledInvitationResponse = await _members.GetMemberInvitation(home.Id, createdMember.Id, invitation.Id);
        cancelledInvitationResponse.Content!.Status.Should().Be(InvitationStatus.Cancelled);

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Delete_invitation_with_invalid_parameters_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var response = await _members.CancelMemberInvitations(Guid.NewGuid(), Guid.NewGuid());
        response.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        response = await _members.CancelMemberInvitations(home.Id, Guid.NewGuid());
        response.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
