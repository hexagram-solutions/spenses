using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(WebApplicationCollection.CollectionName)]
public partial class HomesIntegrationTests(WebApplicationFixture<Program> fixture)
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());

    private readonly IExpenseCategoriesApi _expenseCategories =
        RestService.For<IExpenseCategoriesApi>(fixture.WebApplicationFactory.CreateClient());
}
