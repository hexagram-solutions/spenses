using System.Net;
using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Expenses;
using Spenses.Shared.Models.Members;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Put_home_member_updates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var properties = new MemberProperties
        {
            Name = "Grunky Peep",
            DefaultSplitPercentage = 0.0m,
            ContactEmail = _faker.Internet.Email()
        };

        var updatedMemberResponse = await _members.PutMember(home.Id, member.Id, properties);

        updatedMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedMember = updatedMemberResponse.Content!;

        updatedMember.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        updatedMember.Status.Should().Be(MemberStatus.Active);

        var fetchedMember = (await _members.GetMember(home.Id, member.Id)).Content;
        fetchedMember.Should().BeEquivalentTo(updatedMember);
    }

    [Fact]
    public async Task Put_invalid_member_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var memberId = home.Members.First();

        var result = await _members.PutMember(home.Id, memberId.Id, new MemberProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_home_member_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var homeNotFoundResult = await _members.PutMember(Guid.NewGuid(), member.Id, member);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberNotFoundResult = await _members.PutMember(home.Id, Guid.NewGuid(), member);

        memberNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_home_member_does_not_change_is_active_property_when_member_is_inactive()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expensesApi = CreateApiClient<IExpensesApi>();

        var expenses = await expensesApi.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Take = 1,
            MinDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
            MaxDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
        });

        var memberId = expenses.Content!.Items.First().PaidByMemberId;

        var member = (await _members.DeleteMember(home.Id, memberId)).Content!.Model;

        var updatedMemberResponse = await _members.PutMember(home.Id, memberId, member);

        updatedMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedMember = updatedMemberResponse.Content!;

        updatedMember.Should().BeEquivalentTo(member);

        await _members.ActivateMember(home.Id, member.Id);
    }

    [Fact]
    public async Task Put_home_member_with_duplicate_contact_email_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member1 = home.Members[0];
        var member2 = home.Members[1];

        member2.ContactEmail = member1.ContactEmail;

        var response = await _members.PutMember(home.Id, member2.Id, member2);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        response.Should().HaveValidationErrorFor(x => x.ContactEmail);
    }
}
