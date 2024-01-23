using System.Net;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Post_home_member_creates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new MemberProperties
        {
            Name = "Bob",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = "bob@example.com"
        };

        var createdMemberResponse = await _members.PostMember(home.Id, properties);

        createdMemberResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdMember = createdMemberResponse.Content!;

        createdMember.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        createdMember.IsActive.Should().BeTrue();

        var members = (await _members.GetMembers(home.Id)).Content;
        members.Should().ContainEquivalentOf(createdMember,
            opts => opts.Excluding(u => u.AvatarUrl));

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Post_invalid_member_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _members.PostMember(home.Id, new MemberProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_home_member_with_invalid_identifiers_yields_not_found()
    {
        var homeNotFoundResult = await _members.PostMember(Guid.NewGuid(), new MemberProperties
        {
            Name = "Grunky Peep",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = "grunky.peep@georgiasouthern.edu"
        });

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
