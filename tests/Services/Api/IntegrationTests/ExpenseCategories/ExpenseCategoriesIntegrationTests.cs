using Refit;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.ExpenseCategories;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class ExpenseCategoriesIntegrationTests(IdentityWebApplicationFixture fixture) : IAsyncLifetime
{
    private readonly IExpenseCategoriesApi _categories =
        RestService.For<IExpenseCategoriesApi>(fixture.CreateAuthenticatedClient());

    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());

    public async Task InitializeAsync()
    {
        await fixture.LoginAsTestUser();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
