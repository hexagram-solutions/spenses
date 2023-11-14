using System.Net;
using Spenses.Application.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests
{
    [Fact]
    public async Task Put_expense_updates_expense()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var category = (await _expenseCategories.GetExpenseCategories(home.Id)).Content!.First();

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
            CategoryId = category.Id,
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

    [Fact]
    public async Task Put_expense_with_invalid_paid_by_member_id_yields_bad_request()
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
            PaidByMemberId = Guid.NewGuid()
        };

        var updatedExpenseResponse = await _expenses.PutExpense(home.Id, expense.Id, properties);

        updatedExpenseResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_expense_with_invalid_identifiers_yields_not_found()
    {
        var homeId = (await _homes.GetHomes()).Content!.First().Id;

        var category = (await _expenseCategories.GetExpenseCategories(homeId)).Content!.First();

        var expenseId = (await _expenses.GetExpenses(homeId, new FilteredExpensesQuery())).Content!.Items.First().Id;

        var properties = new ExpenseProperties
        {
            Note = "Bar",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = new[] { "household" },
            CategoryId = category.Id,
            PaidByMemberId = Guid.NewGuid()
        };

        var homeNotFoundResult = await _expenses.PutExpense(Guid.NewGuid(), expenseId, properties);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var expenseNotFoundResult = await _expenses.PutExpense(homeId, Guid.NewGuid(), properties);

        expenseNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
