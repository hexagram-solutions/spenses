using Bogus;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Identity;

public partial class IdentityIntegrationTests : IdentityIntegrationTestBase
{
    private readonly Faker _faker = new();

    private readonly IIdentityApi _identityApi;

    public IdentityIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
        : base(databaseFixture, authFixture)
    {
        _identityApi = CreateApiClient<IIdentityApi>();
    }
}
