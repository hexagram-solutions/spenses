using System.Net;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Get_invitations_with_invalid_parameters_yields_not_found()
    {
        var response = await _members.GetMemberInvitations(Guid.NewGuid(), Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        response = await _members.GetMemberInvitations(homeId, Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberId = (await _members.GetMembers(homeId)).Content!.First().Id;

        response = await _members.GetMemberInvitations(homeId, memberId);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
