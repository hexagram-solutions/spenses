using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

public partial class HomesIntegrationTests : IdentityIntegrationTestBase
{
    private readonly IHomesApi _homes;
    private readonly IExpenseCategoriesApi _expenseCategories;

    public HomesIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
        : base(databaseFixture, authFixture)
    {
        _homes = CreateApiClient<IHomesApi>();
        _expenseCategories = CreateApiClient<IExpenseCategoriesApi>();
    }
}
