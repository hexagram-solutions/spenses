using System.Net;
using Spenses.Application.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests
{
    [Fact]
    public async Task Put_expense_creates_expense()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expense = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Skip = 0,
            Take = 10
        })).Content!.Items.First();

        var properties = new ExpenseProperties
        {
            Note = "Bar",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = new[] { "household" },
            PaidByMemberId = home.Members.First().Id
        };

        var updatedExpenseResponse = await _expenses.PutExpense(home.Id, expense.Id, properties);

        updatedExpenseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedExpense = updatedExpenseResponse.Content;

        updatedExpense.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedExpense = (await _expenses.GetExpense(home.Id, updatedExpense!.Id)).Content!;

        fetchedExpense.Should().BeEquivalentTo(updatedExpense);
        fetchedExpense.ExpenseShares.Select(es => es.OwedByMember).Should().BeEquivalentTo(home.Members);
    }
}
