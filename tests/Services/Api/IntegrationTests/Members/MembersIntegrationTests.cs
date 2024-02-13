using Bogus;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    : IdentityIntegrationTestBase(databaseFixture, authFixture)
{
    private readonly Faker _faker = new();

    private IHomesApi _homes = null!;
    private IMembersApi _members = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _homes = CreateApiClient<IHomesApi>();
        _members = CreateApiClient<IMembersApi>();
    }
}
