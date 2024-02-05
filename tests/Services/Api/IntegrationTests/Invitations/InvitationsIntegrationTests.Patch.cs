using System.Net;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Api.IntegrationTests.Invitations;

public partial class InvitationsIntegrationTests
{
    [Fact]
    public async Task Patch_invitation_accepts_invitation_and_adds_user_to_home()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        var (memberId, _) = await CreateAndInviteMember(home.Id, email);

        var invitationId = fixture.GetInvitationIdForEmail(email);

        // Register and log in as a new user who will accept the invitation
        await RegisterAndLogIn(email);

        // User is not a part of the home yet
        var homes = await _homes.GetHomes();
        homes.Content!.Should().BeEmpty();

        // Accept the invitation
        var invitationResponse = await _invitations.AcceptInvitation(invitationId);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // User should now be a part of the home
        var userHomeResponse = await _homes.GetHome(home.Id);
        userHomeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var memberInvitationsResponse = await _members.GetMemberInvitations(home.Id, memberId);
        memberInvitationsResponse.Content!.Single().Status.Should().Be(InvitationStatus.Accepted);

        await _members.DeleteMember(home.Id, memberId);
        await fixture.DeleteUser(email);
    }

    [Fact]
    public async Task Patch_invitation_that_has_already_been_accepted_yields_success()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        var (memberId, _) = await CreateAndInviteMember(home.Id, email);
        var invitationId = fixture.GetInvitationIdForEmail(email);

        // Register and log in as a new user who will accept the invitation
        await RegisterAndLogIn(email);

        // User is not a part of the home yet
        var homes = await _homes.GetHomes();
        homes.Content!.Should().BeEmpty();

        // Accept the invitation
        var invitationResponse = await _invitations.AcceptInvitation(invitationId);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        invitationResponse = await _invitations.AcceptInvitation(invitationId);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        await _members.DeleteMember(home.Id, memberId);
        await fixture.DeleteUser(email);
    }

    [Fact]
    public async Task Patch_invitation_for_cancelled_invitation_yields_forbidden()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        var (memberId, _) = await CreateAndInviteMember(home.Id, email);

        await _members.CancelMemberInvitations(home.Id, memberId);

        var invitationId = fixture.GetInvitationIdForEmail(email);

        await RegisterAndLogIn(email);

        var invitationResponse = await _invitations.AcceptInvitation(invitationId);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        await _members.DeleteMember(home.Id, memberId);
        await fixture.DeleteUser(email);
    }

    [Fact]
    public async Task Patch_invitation_with_invalid_parameters_yields_bad_request()
    {
        var invitationResponse = await _invitations.AcceptInvitation(Guid.Empty);

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Patch_invitation_for_other_email_address_yields_forbidden()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        var (memberId, _) = await CreateAndInviteMember(home.Id, email);

        var invitationId = fixture.GetInvitationIdForEmail(email);

        var response = await _invitations.AcceptInvitation(invitationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        await _members.DeleteMember(home.Id, memberId);
    }
}
