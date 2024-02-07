using System.Net;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Post_home_member_creates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new CreateMemberProperties
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

        createdMember.Status.Should().Be(MemberStatus.Active);

        var members = (await _members.GetMembers(home.Id)).Content;
        members.Should().ContainEquivalentOf(createdMember,
            opts => opts.Excluding(u => u.AvatarUrl));

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Post_invalid_member_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _members.PostMember(home.Id, new CreateMemberProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_home_member_with_invalid_identifiers_yields_not_found()
    {
        var homeNotFoundResult = await _members.PostMember(Guid.NewGuid(), new CreateMemberProperties
        {
            Name = "Grunky Peep",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = "grunky.peep@georgiasouthern.edu"
        });

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_home_member_with_should_invite_set_sends_invitation()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        const string email = "quatro.quatro@sjsu.edu";

        var properties = new CreateMemberProperties
        {
            Name = "Quatro Quatro",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = email,
            ShouldInvite = true
        };

        var createdMemberResponse = await _members.PostMember(home.Id, properties);

        createdMemberResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdMember = createdMemberResponse.Content!;

        createdMember.Status.Should().Be(MemberStatus.Invited);

        var invitationMessage = fixture.GetLastMessageForEmail(email);

        invitationMessage.RecipientAddress.Should().Be(email);
        invitationMessage.Subject.Should().Contain(home.Name);
        invitationMessage.PlainTextMessage.Should().Contain("?invitationToken=");

        await _members.DeleteMember(home.Id, createdMember.Id);
    }

    [Fact]
    public async Task Post_home_member_with_should_invite_and_no_contact_email_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new CreateMemberProperties
        {
            Name = "Quatro Quatro",
            DefaultSplitPercentage = 0.0m,
            ShouldInvite = true
        };

        var response = await _members.PostMember(home.Id, properties);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        response.Should().HaveValidationErrorFor(x => x.ContactEmail);
    }
}
