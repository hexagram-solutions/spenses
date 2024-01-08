using System.Net;
using Spenses.Shared.Models.Payments;

namespace Spenses.Api.IntegrationTests.Payments;

public partial class PaymentsIntegrationTests
{
    [Fact]
    public async Task Put_payment_creates_payment()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var payment = (await _payments.GetPayments(home.Id, new FilteredPaymentQuery
        {
            Skip = 0,
            Take = 100
        })).Content!.Items.First();

        var properties = new PaymentProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Note = "foobar",
            PaidByMemberId = home.Members[0].Id,
            PaidToMemberId = home.Members[1].Id
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
    public async Task Put_payment_with_invalid_paid_by_member_yields_bad_request()
    {

        var home = (await _homes.GetHomes()).Content!.First();

        var payment = (await _payments.GetPayments(home.Id, new FilteredPaymentQuery
        {
            Skip = 0,
            Take = 100
        })).Content!.Items.First();

        var properties = new PaymentProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Note = "foobar",
            PaidByMemberId = Guid.NewGuid(),
            PaidToMemberId = home.Members[1].Id
        };

        var updatedPaymentResponse = await _payments.PutPayment(home.Id, payment.Id, properties);

        updatedPaymentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_payment_with_invalid_paid_to_member_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var payment = (await _payments.GetPayments(home.Id, new FilteredPaymentQuery
        {
            Skip = 0,
            Take = 100
        })).Content!.Items.First();

        var properties = new PaymentProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Note = "foobar",
            PaidByMemberId = home.Members[0].Id,
            PaidToMemberId = Guid.NewGuid()
        };

        var updatedPaymentResponse = await _payments.PutPayment(home.Id, payment.Id, properties);

        updatedPaymentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_expense_with_invalid_identifiers_yields_not_found()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var paymentId = (await _payments.GetPayments(home.Id, new FilteredPaymentQuery())).Content!.Items.First().Id;

        var properties = new PaymentProperties
        {
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Note = "foobar",
            PaidByMemberId = home.Members[0].Id,
            PaidToMemberId = Guid.NewGuid()
        };

        var homeNotFoundResult = await _payments.PutPayment(Guid.NewGuid(), paymentId, properties);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var paymentNotFoundResult = await _payments.PutPayment(home.Id, Guid.NewGuid(), properties);

        paymentNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
