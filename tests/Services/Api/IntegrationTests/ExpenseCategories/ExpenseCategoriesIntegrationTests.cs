using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class ExpenseCategoriesIntegrationTests(IdentityWebApplicationFixture<Program> fixture)
{
    private readonly IExpenseCategoriesApi _categories =
        RestService.For<IExpenseCategoriesApi>(fixture.CreateClient());

    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateClient());
}
