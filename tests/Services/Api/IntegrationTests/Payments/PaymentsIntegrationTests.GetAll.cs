using System.Net;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Payments;

namespace Spenses.Api.IntegrationTests.Payments;

public partial class PaymentsIntegrationTests
{
    [Fact]
    public async Task Get_payments_with_invalid_identifiers_yields_not_found()
    {
        var homeNotFoundResult = await _payments.GetPayments(Guid.NewGuid(), new FilteredPaymentQuery());

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_payments_with_period_filters_yields_payments_in_range()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var unfilteredPayments =
            (await _payments.GetPayments(home.Id, new FilteredPaymentQuery { Skip = 0, Take = 100 })).Content!
            .Items.ToList();

        var earliestPaymentDate = unfilteredPayments.MinBy(x => x.Date)!.Date;
        var latestPaymentDate = unfilteredPayments.MaxBy(x => x.Date)!.Date;

        var minDateFilterValue = earliestPaymentDate.AddDays(1);
        var maxDateFilterValue = latestPaymentDate.AddDays(-1);

        var filteredPayments = (await _payments.GetPayments(home.Id,
            new FilteredPaymentQuery
            {
                Skip = 0,
                Take = 100,
                MinDate = minDateFilterValue,
                MaxDate = maxDateFilterValue
            })).Content!.Items;

        filteredPayments.Should().AllSatisfy(e =>
        {
            e.Date.Should().BeOnOrAfter(minDateFilterValue)
                .And.BeOnOrBefore(maxDateFilterValue);
        });
    }

    [Fact]
    public async Task Get_payments_ordered_by_amount_yields_ordered_payments()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var query = new FilteredPaymentQuery
        {
            Skip = 0,
            Take = 25,
            OrderBy = nameof(PaymentDigest.Amount),
            SortDirection = SortDirection.Asc
        };

        var payments = (await _payments.GetPayments(home.Id, query)).Content!.Items;

        payments.Should().BeInAscendingOrder(x => x.Amount);

        payments = (await _payments.GetPayments(home.Id, query with { SortDirection = SortDirection.Desc }))
            .Content!.Items;

        payments.Should().BeInDescendingOrder(x => x.Amount);
    }
}
