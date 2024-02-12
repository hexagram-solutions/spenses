using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

public partial class ExpenseCategoriesIntegrationTests : IdentityIntegrationTestBase
{
    private readonly IExpenseCategoriesApi _categories;
    private readonly IHomesApi _homes;

    public ExpenseCategoriesIntegrationTests(IdentityWebApplicationFixture fixture) : base(fixture)
    {
        _categories = CreateApiClient<IExpenseCategoriesApi>();
        _homes = CreateApiClient<IHomesApi>();
    }
}
