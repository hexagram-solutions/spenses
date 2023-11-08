using System.Net;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Get_all_members_yields_members_ordered_by_name()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var membersResponse = await _members.GetMembers(homeId);

        membersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        membersResponse.Content.Should().BeInAscendingOrder(x => x.Name, StringComparer.InvariantCulture);
    }

    [Fact]
    public async Task Get_all_members_with_invalid_identifiers_yields_not_found()
    {
        var homeNotFoundResult = await _members.GetMembers(Guid.NewGuid());

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
