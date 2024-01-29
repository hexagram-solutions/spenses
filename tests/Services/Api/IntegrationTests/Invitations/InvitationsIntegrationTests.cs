using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Invitations;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class InvitationsIntegrationTests(IdentityWebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateClient());
    private readonly IMembersApi _members = RestService.For<IMembersApi>(fixture.CreateClient());
    private readonly IInvitationsApi _invitations = RestService.For<IInvitationsApi>(fixture.CreateClient());
}
