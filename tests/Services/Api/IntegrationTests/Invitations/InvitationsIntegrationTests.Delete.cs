using System.Net;
using Microsoft.EntityFrameworkCore;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Api.IntegrationTests.Invitations;

public partial class InvitationsIntegrationTests
{
    [Fact]
    public async Task Delete_invitation_declines_invitation()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = AuthFixture.VerifiedUser.Email;

        await CreateAndInviteMember(home.Id, email);

        var invitationId = AuthFixture.GetInvitationIdForEmail(email);

        var invitationResponse = await _invitations.DeclineInvitation(invitationId);

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await DatabaseFixture.ExecuteDbContextAction(async db =>
        {
            var invitation = await db.Invitations
                .Include(i => i.Member)
                .SingleAsync(i => i.Id == invitationId);

            invitation.Status.Should().Be(DbModels.InvitationStatus.Declined);
            invitation.Member.Status.Should().Be(DbModels.MemberStatus.Active);
        });
    }

    [Fact]
    public async Task Delete_invitation_with_invalid_parameters_yields_not_found()
    {
        var result = await _invitations.DeclineInvitation(Guid.NewGuid());

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_accepted_invitation_yields_forbidden()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        await CreateAndInviteMember(home.Id, email);

        var invitationId = AuthFixture.GetInvitationIdForEmail(email);

        await RegisterAndLogIn(email);

        var invitationAcceptedResponse = await _invitations.AcceptInvitation(invitationId);
        invitationAcceptedResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var invitationDeclinedResponse = await _invitations.DeclineInvitation(invitationId);
        invitationDeclinedResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_invitation_that_has_already_been_declined_yields_success()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = AuthFixture.VerifiedUser.Email;

        await CreateAndInviteMember(home.Id, email);

        var invitationId = AuthFixture.GetInvitationIdForEmail(email);

        var invitationResponse = await _invitations.DeclineInvitation(invitationId);

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        invitationResponse = await _invitations.DeclineInvitation(invitationId);

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_invitation_for_other_email_address_yields_forbidden()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        await CreateAndInviteMember(home.Id, email);

        var invitationId = AuthFixture.GetInvitationIdForEmail(email);

        var response = await _invitations.DeclineInvitation(invitationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
