using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class HomesIntegrationTests(IdentityWebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateClient());

    private readonly IExpenseCategoriesApi _expenseCategories =
        RestService.For<IExpenseCategoriesApi>(fixture.CreateClient());
}
