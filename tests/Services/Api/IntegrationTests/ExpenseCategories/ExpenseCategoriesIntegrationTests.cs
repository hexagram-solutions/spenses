using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

public partial class ExpenseCategoriesIntegrationTests(
    DatabaseFixture databaseFixture,
    AuthenticationFixture authFixture)
    : IdentityIntegrationTestBase(databaseFixture, authFixture)
{
    private IExpenseCategoriesApi _categories = null!;
    private IHomesApi _homes = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _categories = CreateApiClient<IExpenseCategoriesApi>();
        _homes = CreateApiClient<IHomesApi>();
    }
}
