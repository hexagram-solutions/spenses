using System.Net;
using Spenses.Application.Models.Payments;

namespace Spenses.Api.IntegrationTests.Payments;

public partial class PaymentsIntegrationTests
{
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
}
