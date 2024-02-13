using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

public partial class HomesIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    : IdentityIntegrationTestBase(databaseFixture, authFixture)
{
    private IHomesApi _homes = null!;
    private IExpenseCategoriesApi _expenseCategories = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _homes = CreateApiClient<IHomesApi>();
        _expenseCategories = CreateApiClient<IExpenseCategoriesApi>();
    }
}
