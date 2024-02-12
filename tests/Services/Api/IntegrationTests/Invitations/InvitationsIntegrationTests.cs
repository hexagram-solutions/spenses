using Bogus;
using Spenses.Client.Http;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Invitations;

public partial class InvitationsIntegrationTests : IdentityIntegrationTestBase
{
    private readonly IHomesApi _homes;
    private readonly IMembersApi _members;
    private readonly IInvitationsApi _invitations;

    public InvitationsIntegrationTests(IdentityWebApplicationFixture fixture) : base(fixture)
    {
        _homes = CreateApiClient<IHomesApi>();
        _members = CreateApiClient<IMembersApi>();
        _invitations = CreateApiClient<IInvitationsApi>();
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

        await Register(registerRequest, true);

        await Login(new LoginRequest { Email = email, Password = registerRequest.Password });
    }
}
