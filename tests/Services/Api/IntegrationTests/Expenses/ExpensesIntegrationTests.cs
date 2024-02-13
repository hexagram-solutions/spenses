using Spenses.Client.Http;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests : IdentityIntegrationTestBase
{
    private readonly IHomesApi _homes;
    private readonly IExpensesApi _expenses;
    private readonly IExpenseCategoriesApi _expenseCategories;

    public ExpensesIntegrationTests(DatabaseFixture databaseFixture, AuthenticationFixture authFixture)
        : base(databaseFixture, authFixture)
    {
        _homes = CreateApiClient<IHomesApi>();
        _expenses = CreateApiClient<IExpensesApi>();
        _expenseCategories = CreateApiClient<IExpenseCategoriesApi>();
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
