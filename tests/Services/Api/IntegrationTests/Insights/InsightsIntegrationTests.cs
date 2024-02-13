using System.Net;
using Spenses.Client.Http;
using Spenses.Shared.Models.Insights;

namespace Spenses.Api.IntegrationTests.Insights;

public class InsightsIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    : IdentityIntegrationTestBase(databaseFixture, authFixture)
{
    private IHomesApi _homes = null!;
    private IInsightsApi _insights = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _homes = CreateApiClient<IHomesApi>();
        _insights = CreateApiClient<IInsightsApi>();
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
