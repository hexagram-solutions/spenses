using System.Net;

namespace Spenses.Api.IntegrationTests.Payments;

public partial class PaymentsIntegrationTests
{
    [Fact]
    public async Task Get_payment_with_invalid_identifiers_yields_not_found()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var paymentId = (await _payments.GetPayments(homeId, DefaultPaymentsQuery)).Content!.Items.First().Id;

        var homeNotFoundResult = await _payments.DeletePayment(Guid.NewGuid(), paymentId);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var paymentNotFoundResult = await _payments.DeletePayment(homeId, Guid.NewGuid());

        paymentNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
