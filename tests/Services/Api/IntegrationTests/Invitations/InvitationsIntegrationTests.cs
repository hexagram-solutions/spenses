using Bogus;
using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Invitations;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class InvitationsIntegrationTests(IdentityWebApplicationFixture<Program> fixture) : IAsyncLifetime
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());
    private readonly IMembersApi _members = RestService.For<IMembersApi>(fixture.CreateAuthenticatedClient());
    private readonly IInvitationsApi _invitations = RestService.For<IInvitationsApi>(fixture.CreateAuthenticatedClient());

    public async Task InitializeAsync()
    {
        await fixture.LoginAsTestUser();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
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
