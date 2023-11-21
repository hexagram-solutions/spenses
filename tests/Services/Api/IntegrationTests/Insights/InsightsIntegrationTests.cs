using System.Net;
using Refit;
using Spenses.Application.Models.Insights;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Insights;

[Collection(WebApplicationCollection.CollectionName)]
public class InsightsIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());

    private readonly IInsightsApi _insights =
        RestService.For<IInsightsApi>(fixture.WebApplicationFactory.CreateClient());

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
