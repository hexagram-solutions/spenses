using System.Net;
using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Insights;

namespace Spenses.Api.IntegrationTests.Insights;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public class InsightsIntegrationTests(IdentityWebApplicationFixture fixture) : IAsyncLifetime
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());

    private readonly IInsightsApi _insights =
        RestService.For<IInsightsApi>(fixture.CreateAuthenticatedClient());

    public async Task InitializeAsync()
    {
        await fixture.LoginAsTestUser();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(ExpenseDateGrouping.Month)]
    [InlineData(ExpenseDateGrouping.Year)]
    public async Task Get_expenses_over_time_yields_success(ExpenseDateGrouping grouping)
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var dataResult = await _insights.GetExpensesOverTime(homeId, grouping);

        dataResult.StatusCode.Should().Be(HttpStatusCode.OK);

        dataResult.Content!.Should().AllSatisfy(x =>
        {
            switch (grouping)
            {
                case ExpenseDateGrouping.Month:
                    x.Date.Day.Should().Be(1);
                    break;

                case ExpenseDateGrouping.Year:
                    x.Date.Day.Should().Be(1);
                    x.Date.Month.Should().Be(1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(grouping), grouping, null);
            }
        });
    }
}
