using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

[Collection(IdentityWebApplicationCollection.CollectionName)]
public partial class ExpensesIntegrationTests(IdentityWebApplicationFixture<Program> fixture) : IAsyncLifetime
{
    private readonly IHomesApi _homes = RestService.For<IHomesApi>(fixture.CreateAuthenticatedClient());

    private readonly IExpensesApi _expenses = RestService.For<IExpensesApi>(fixture.CreateAuthenticatedClient(),
        new RefitSettings { CollectionFormat = CollectionFormat.Multi });

    private readonly IExpenseCategoriesApi _expenseCategories = RestService.For<IExpenseCategoriesApi>(fixture.CreateAuthenticatedClient(),
        new RefitSettings { CollectionFormat = CollectionFormat.Multi });

    public async Task InitializeAsync()
    {
        await fixture.LoginAsTestUser();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private FilteredExpensesQuery DefaultExpensesQuery
    {
        get
        {
            var today = DateTime.Today;

            var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

            return new FilteredExpensesQuery
            {
                OrderBy = nameof(ExpenseDigest.Date),
                SortDirection = SortDirection.Desc,
                MinDate = new DateOnly(today.Year, today.Month, 1),
                MaxDate = new DateOnly(today.Year, today.Month, daysInMonth)
            };
        }
    }
}
