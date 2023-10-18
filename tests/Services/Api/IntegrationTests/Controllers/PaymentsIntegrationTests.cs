using System.Net;
using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Payments;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Controllers;

[Collection(WebApplicationCollection.CollectionName)]
public class PaymentsIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IPaymentsApi _payments;

    public PaymentsIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
        _payments = RestService.For<IPaymentsApi>(fixture.WebApplicationFactory.CreateClient());
    }

    [Fact]
    public async Task Post_payment_creates_payment()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new PaymentProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Note = "foobar",
            PaidByMemberId = home.Members.First().Id
        };

        var createdPaymentResponse = await _payments.PostPayment(home.Id, properties);

        createdPaymentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdPayment = createdPaymentResponse.Content;

        createdPayment.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedPayment = (await _payments.GetPayment(home.Id, createdPayment!.Id)).Content;
        fetchedPayment.Should().BeEquivalentTo(createdPayment);

        await _payments.DeletePayment(home.Id, createdPayment.Id);
    }

    [Fact]
    public async Task Post_invalid_payment_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _payments.PostPayment(home.Id, new PaymentProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_payment_creates_payment()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var payment = (await _payments.GetPayments(home.Id, new FilteredPaymentQuery
        {
            PageNumber = 1,
            PageSize = 100
        })).Content!.Items.First();

        var properties = new PaymentProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Note = "foobar",
            PaidByMemberId = home.Members.First().Id
        };

        var updatedPaymentResponse = await _payments.PutPayment(home.Id, payment.Id, properties);

        updatedPaymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedPayment = updatedPaymentResponse.Content;

        updatedPayment.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedPayment = (await _payments.GetPayment(home.Id, updatedPayment!.Id)).Content;

        fetchedPayment.Should().BeEquivalentTo(updatedPayment);
    }

    [Fact]
    public async Task Get_payments_with_period_filters_yields_Payments_in_range()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var unfilteredPayments = (await _payments.GetPayments(home.Id, new FilteredPaymentQuery
        {
            PageNumber = 1,
            PageSize = 100
        })).Content!.Items.ToList();

        var earliestPaymentDate = unfilteredPayments.MinBy(x => x.Date)!.Date;
        var latestPaymentDate = unfilteredPayments.MaxBy(x => x.Date)!.Date;

        var minDateFilterValue = earliestPaymentDate.AddDays(1);
        var maxDateFilterValue = latestPaymentDate.AddDays(-1);

        var filteredPayments = (await _payments.GetPayments(home.Id, new FilteredPaymentQuery
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
