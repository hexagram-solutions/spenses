using System.Net;
using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests
{
    [Fact]
    public async Task Activate_inactive_member_activates_member()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        // Get a member using expenses in the home to ensure we get a member with associations to deactivate
        var expensesApi = RestService.For<IExpensesApi>(fixture.CreateAuthenticatedClient());

        var expenses = await expensesApi.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Take = 1,
            MinDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
            MaxDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
        });

        var memberId = expenses.Content!.Items.First().PaidByMemberId;

        var member = (await _members.GetMember(home.Id, memberId)).Content!;

        var deleteMemberResponse = await _members.DeleteMember(home.Id, memberId);

        deleteMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var activateMemberResponse = await _members.ActivateMember(home.Id, memberId);

        activateMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        activateMemberResponse.Content.Should().BeEquivalentTo(member, opts => opts.Excluding(m => m.AvatarUrl));
    }

    [Fact]
    public async Task Activate_currently_active_member_yields_success()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var activateMemberResponse = await _members.ActivateMember(home.Id, member.Id);

        activateMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        activateMemberResponse.Content.Should().BeEquivalentTo(member, opts => opts.Excluding(m => m.AvatarUrl));
    }

    [Fact]
    public async Task Activate_home_member_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var member = home.Members.First();

        var homeNotFoundResult = await _members.ActivateMember(Guid.NewGuid(), member.Id);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var memberNotFoundResult = await _members.ActivateMember(home.Id, Guid.NewGuid());

        memberNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
