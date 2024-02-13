using Spenses.Client.Http;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
    : IdentityIntegrationTestBase(databaseFixture, authFixture)
{
    private IExpenseCategoriesApi _expenseCategories = null!;
    private IExpensesApi _expenses = null!;
    private IHomesApi _homes = null!;

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

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _homes = CreateApiClient<IHomesApi>();
        _expenses = CreateApiClient<IExpensesApi>();
        _expenseCategories = CreateApiClient<IExpenseCategoriesApi>();
    }
}
