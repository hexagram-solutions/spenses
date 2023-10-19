using Spenses.Application.Models.Common;
using Spenses.Application.Models.Payments;

namespace Spenses.Api.IntegrationTests.Payments;

public partial class PaymentsIntegrationTests
{
    [Fact]
    public async Task Get_payments_with_period_filters_yields_Payments_in_range()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var unfilteredPayments =
            (await _payments.GetPayments(home.Id, new FilteredPaymentQuery { PageNumber = 1, PageSize = 100 })).Content!
            .Items.ToList();

        var earliestPaymentDate = unfilteredPayments.MinBy(x => x.Date)!.Date;
        var latestPaymentDate = unfilteredPayments.MaxBy(x => x.Date)!.Date;

        var minDateFilterValue = earliestPaymentDate.AddDays(1);
        var maxDateFilterValue = latestPaymentDate.AddDays(-1);

        var filteredPayments = (await _payments.GetPayments(home.Id,
            new FilteredPaymentQuery
            {
                PageNumber = 1,
                PageSize = 100,
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
    public async Task Get_payments_ordered_by_amount_yields_ordered_Payments()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var query = new FilteredPaymentQuery
        {
            PageNumber = 1,
            PageSize = 25,
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
