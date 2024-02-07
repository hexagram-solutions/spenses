using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Identity;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class IdentityIntegrationTests(IdentityWebApplicationFixture<Program> fixture)
{
    private readonly IIdentityApi _identityApi =
        RestService.For<IIdentityApi>(fixture.CreateAuthenticatedClient());
}
