using System.Net;
using Spenses.Application.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests
{
    [Fact]
    public async Task Post_expense_creates_expense()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new ExpenseProperties
        {
            Note = "Foo",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = new[] { "groceries" },
            PaidByMemberId = home.Members.First().Id
        };

        var createdExpenseResponse = await _expenses.PostExpense(home.Id, properties);

        createdExpenseResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdExpense = createdExpenseResponse.Content;

        createdExpense.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedExpense = (await _expenses.GetExpense(home.Id, createdExpense!.Id)).Content!;
        fetchedExpense.Should().BeEquivalentTo(createdExpense);
        fetchedExpense.ExpenseShares.Select(es => es.OwedByMember).Should().BeEquivalentTo(home.Members);

        await _expenses.DeleteExpense(home.Id, createdExpense.Id);
    }

    [Fact]
    public async Task Post_invalid_expense_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _expenses.PostExpense(home.Id, new ExpenseProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
