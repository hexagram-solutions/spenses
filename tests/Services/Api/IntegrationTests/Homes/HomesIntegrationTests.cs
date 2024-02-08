using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class HomesIntegrationTests(IdentityWebApplicationFixture<Program> fixture) : IAsyncLifetime
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());
    private readonly IExpenseCategoriesApi _expenseCategories =
        RestService.For<IExpenseCategoriesApi>(fixture.CreateAuthenticatedClient());

    public async Task InitializeAsync()
    {
        await fixture.LoginAsTestUser();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
