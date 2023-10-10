using System.Net;
using Refit;
using Spenses.Application.Models.Members;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Controllers;

[Collection(WebApplicationCollection.CollectionName)]
public class MembersIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IMembersApi _members;

    public MembersIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
        _members = RestService.For<IMembersApi>(fixture.WebApplicationFactory.CreateClient());
    }

    [Fact]
    public async Task Post_home_member_creates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new MemberProperties
        {
            Name = "Bob",
            SplitPercentage = 0.0,
            ContactEmail = "bob@example.com"
        };

        var createdMemberResponse = await _members.PostMember(home.Id, properties);

        createdMemberResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdMember = createdMemberResponse.Content;

        createdMember.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var members = (await _members.GetMembers(home.Id)).Content;
        members.Should().ContainEquivalentOf(createdMember);

        await _members.DeleteMember(home.Id, createdMember!.Id);
    }

    [Fact]
    public async Task Post_invalid_member_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _members.PostMember(home.Id, new MemberProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_home_member_updates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var properties = new MemberProperties
        {
            Name = "Grunky Peep",
            SplitPercentage = 0.0,
            ContactEmail = "grunky.peep@georgiasouthern.edu"
        };

        var updatedMemberResponse = await _members.PutMember(home.Id, member.Id, properties);

        updatedMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedMember = updatedMemberResponse.Content;

        updatedMember.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedMember = (await _members.GetMember(home.Id, member.Id)).Content;
        fetchedMember.Should().BeEquivalentTo(updatedMember);
    }

    [Fact]
    public async Task Get_home_member_with_invalid_id_returns_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _members.GetMember(home.Id, Guid.Empty);

        result.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
