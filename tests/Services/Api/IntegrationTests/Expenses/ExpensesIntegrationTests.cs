using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Expenses;

[Collection(WebApplicationCollection.CollectionName)]
public partial class ExpensesIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());

    private readonly IExpensesApi _expenses = RestService.For<IExpensesApi>(fixture.WebApplicationFactory.CreateClient(),
        new RefitSettings { CollectionFormat = CollectionFormat.Multi });

    private readonly IExpenseCategoriesApi _expenseCategories = RestService.For<IExpenseCategoriesApi>(fixture.WebApplicationFactory.CreateClient(),
        new RefitSettings { CollectionFormat = CollectionFormat.Multi });
}
