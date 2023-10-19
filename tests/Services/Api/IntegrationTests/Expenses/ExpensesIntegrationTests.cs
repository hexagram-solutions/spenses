using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Expenses;

[Collection(WebApplicationCollection.CollectionName)]
public partial class ExpensesIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IExpensesApi _expenses;
    private readonly IExpenseCategoriesApi _expenseCategories;

    public ExpensesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());

        _expenses = RestService.For<IExpensesApi>(fixture.WebApplicationFactory.CreateClient(),
            new RefitSettings { CollectionFormat = CollectionFormat.Multi });

        _expenseCategories = RestService.For<IExpenseCategoriesApi>(fixture.WebApplicationFactory.CreateClient(),
            new RefitSettings { CollectionFormat = CollectionFormat.Multi });
    }
}
