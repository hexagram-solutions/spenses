using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

[Collection(WebApplicationCollection.CollectionName)]
public partial class ExpenseCategoriesIntegrationTests
{
    private readonly IExpenseCategoriesApi _categories;
    private readonly IHomesApi _homes;

    public ExpenseCategoriesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _categories = RestService.For<IExpenseCategoriesApi>(fixture.WebApplicationFactory.CreateClient());
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
    }
}
