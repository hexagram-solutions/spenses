using System.Net;
using Bogus;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Invitations;

public partial class InvitationsIntegrationTests
{
    [Fact]
    public async Task Patch_invitation_accepts_invitation_and_adds_user_to_home()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        var (memberId, _) = await CreateAndInviteMember(home.Id, email);

        var token = fixture.GetInvitationTokenForEmail(email);

        // Register and log in as a new user who will accept the invitation
        await RegisterAndLogIn(email);

        // User is not a part of the home yet
        var homes = await _homes.GetHomes();
        homes.Content!.Should().BeEmpty();

        // Accept the invitation
        var invitationResponse = await _invitations.AcceptInvitation(token);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // User should now be a part of the home
        var userHomeResponse = await _homes.GetHome(home.Id);
        userHomeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var memberInvitationsResponse = await _members.GetMemberInvitations(home.Id, memberId);
        memberInvitationsResponse.Content!.Single().Status.Should().Be(InvitationStatus.Accepted);

        await _members.DeleteMember(home.Id, memberId);
        await fixture.DeleteUser(email);
        await fixture.LoginAsTestUser();
    }

    [Fact]
    public async Task Patch_invitation_that_has_already_been_accepted_yields_success()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        var (memberId, _) = await CreateAndInviteMember(home.Id, email);
        var token = fixture.GetInvitationTokenForEmail(email);

        // Register and log in as a new user who will accept the invitation
        await RegisterAndLogIn(email);

        // User is not a part of the home yet
        var homes = await _homes.GetHomes();
        homes.Content!.Should().BeEmpty();

        // Accept the invitation
        var invitationResponse = await _invitations.AcceptInvitation(token);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        invitationResponse = await _invitations.AcceptInvitation(token);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await _members.DeleteMember(home.Id, memberId);
        await fixture.DeleteUser(email);
        await fixture.LoginAsTestUser();
    }

    [Fact]
    public async Task Patch_invitation_for_cancelled_invitation_yields_forbidden()
    {
        var home = (await _homes.GetHomes()).Content!.First();
        var email = "quatro.quatro@sjsu.edu";

        var (memberId, invitationId) = await CreateAndInviteMember(home.Id, email);

        await _members.CancelMemberInvitation(home.Id, memberId, invitationId);

        var token = fixture.GetInvitationTokenForEmail(email);

        await RegisterAndLogIn(email);

        var invitationResponse = await _invitations.AcceptInvitation(token);
        invitationResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        await _members.DeleteMember(home.Id, memberId);
        await fixture.DeleteUser(email);
        await fixture.LoginAsTestUser();
    }

    [Fact]
    public async Task Patch_invitation_with_invalid_token_yields_unauthorized()
    {
        var invitationResponse = await _invitations.AcceptInvitation("foobar");

        invitationResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<(Guid memberId, Guid invitationid)> CreateAndInviteMember(Guid homeId, string email)
    {
        var properties = new CreateMemberProperties
        {
            Name = "Quatro Quatro",
            DefaultSplitPercentage = 0.0m,
        };

        var createdMember = (await _members.PostMember(homeId, properties)).Content!;

        var createdInvitation = (await _members.PostMemberInvitation(
                homeId, createdMember.Id, new InvitationProperties { Email = email }))
            .Content!;

        return (createdMember.Id, createdInvitation.Id);
    }

    private async Task RegisterAndLogIn(string email)
    {
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = new Faker().Internet.Password(),
            DisplayName = "Quatro Quatro"
        };

        await fixture.Register(registerRequest, true);

        await fixture.Login(new LoginRequest { Email = email, Password = registerRequest.Password });
    }
}
