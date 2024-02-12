using Bogus;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Members;

public partial class MembersIntegrationTests : IdentityIntegrationTestBase
{
    private readonly Faker _faker = new();

    private readonly IHomesApi _homes;
    private readonly IMembersApi _members;

    public MembersIntegrationTests(IdentityWebApplicationFixture fixture) : base(fixture)
    {
        _homes = CreateApiClient<IHomesApi>();
        _members = CreateApiClient<IMembersApi>();
    }
}
