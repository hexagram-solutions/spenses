using System.Net;
using Spenses.Application.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests
{
    [Fact]
    public async Task Get_expense_with_invalid_identifiers_yields_not_found()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var expenseId = (await _expenses.GetExpenses(homeId, new FilteredExpensesQuery())).Content!.Items.First().Id;

        var homeNotFoundResult = await _expenses.GetExpense(Guid.NewGuid(), expenseId);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var expenseNotFoundResult = await _expenses.GetExpense(homeId, Guid.NewGuid());

        expenseNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}