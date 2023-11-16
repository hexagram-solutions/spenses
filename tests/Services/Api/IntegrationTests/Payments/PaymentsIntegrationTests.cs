using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Payments;

[Collection(WebApplicationCollection.CollectionName)]
public partial class PaymentsIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());

    private readonly IPaymentsApi _payments =
        RestService.For<IPaymentsApi>(fixture.WebApplicationFactory.CreateClient());
}
