using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Payments;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class PaymentsIntegrationTests(IdentityWebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());

    private readonly IPaymentsApi _payments =
        RestService.For<IPaymentsApi>(fixture.CreateAuthenticatedClient());
}
