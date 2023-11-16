using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

[Collection(WebApplicationCollection.CollectionName)]
public partial class ExpenseCategoriesIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IExpenseCategoriesApi _categories =
        RestService.For<IExpenseCategoriesApi>(fixture.WebApplicationFactory.CreateClient());

    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
}
