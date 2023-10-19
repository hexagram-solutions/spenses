using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Payments;

[Collection(WebApplicationCollection.CollectionName)]
public partial class PaymentsIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IPaymentsApi _payments;

    public PaymentsIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
        _payments = RestService.For<IPaymentsApi>(fixture.WebApplicationFactory.CreateClient());
    }
}
