using System.Net;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Get_invitation_with_invalid_parameters_yields_not_found()
    {
        var response = await _members.GetMemberInvitation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        response = await _members.GetMemberInvitation(homeId, Guid.NewGuid(), Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberResponse = await _members.PostMember(homeId,
            new CreateMemberProperties { Name = "Tyroil Smoochie Wallace", DefaultSplitPercentage = 0.00m });

        var member = memberResponse.Content!;

        var invitationProperties = new InvitationProperties { Email = "foo@bar.com" };

        var invitationResponse = await _members.PostMemberInvitation(homeId, member.Id, invitationProperties);

        response = await _members.GetMemberInvitation(homeId, member.Id, Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        response = await _members.GetMemberInvitation(homeId, member.Id, invitationResponse.Content!.Id);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Should().BeEquivalentTo(invitationProperties);
    }
}
