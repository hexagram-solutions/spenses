using System.Net;
using Microsoft.EntityFrameworkCore;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Delete_invitation_that_is_already_cancelled_yields_success()
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

        var cancelInvitationResponse = await _members.CancelMemberInvitation(home.Id, createdMember.Id, invitation.Id);

        cancelInvitationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        cancelInvitationResponse.Content!.Status.Should().Be(InvitationStatus.Cancelled);

        cancelInvitationResponse = await _members.CancelMemberInvitation(home.Id, createdMember.Id, invitation.Id);

        cancelInvitationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        cancelInvitationResponse.Content!.Status.Should().Be(InvitationStatus.Cancelled);

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Delete_invitation_that_has_been_accepted_yields_bad_request()
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

        await fixture.ExecuteDbContextAction(async db =>
        {
            var dbInvitation = await db.Invitations.SingleAsync(i => i.Id == invitation.Id);
            dbInvitation.Status = DbModels.InvitationStatus.Accepted;
            await db.SaveChangesAsync();
        });

        var cancelInvitationResponse = await _members.CancelMemberInvitation(home.Id, createdMember.Id, invitation.Id);

        cancelInvitationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Delete_invitation_with_invalid_parameters_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var response = await _members.CancelMemberInvitation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        response.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        response = await _members.CancelMemberInvitation(home.Id, Guid.NewGuid(), Guid.NewGuid());
        response.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        response = await _members.CancelMemberInvitation(home.Id, member.Id, Guid.NewGuid());
        response.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
