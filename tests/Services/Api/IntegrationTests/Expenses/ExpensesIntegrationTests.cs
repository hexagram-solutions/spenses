using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Expenses;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class ExpensesIntegrationTests(IdentityWebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());

    private readonly IExpensesApi _expenses = RestService.For<IExpensesApi>(fixture.CreateAuthenticatedClient(),
        new RefitSettings { CollectionFormat = CollectionFormat.Multi });

    private readonly IExpenseCategoriesApi _expenseCategories = RestService.For<IExpenseCategoriesApi>(fixture.CreateAuthenticatedClient(),
        new RefitSettings { CollectionFormat = CollectionFormat.Multi });
}
