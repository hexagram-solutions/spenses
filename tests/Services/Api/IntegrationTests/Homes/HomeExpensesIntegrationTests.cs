using System.Net;
using Refit;
using Spenses.Application.Models;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Homes;

[Collection(WebApplicationCollection.CollectionName)]
public class HomeExpensesIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IHomeExpensesApi _homeExpenses;

    public HomeExpensesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());
        _homeExpenses = RestService.For<IHomeExpensesApi>(fixture.WebApplicationFactory.CreateClient());
    }

    [Fact]
    public async Task Post_expense_creates_expense()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var properties = new ExpenseProperties
        {
            Description = "Foo",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = new []{ "groceries" },
            IncurredByMemberId = home.Members.First().Id
        };

        var createdExpenseResponse = await _homeExpenses.PostHomeExpense(home.Id, properties);

        createdExpenseResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdExpense = createdExpenseResponse.Content;

        createdExpense.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedExpense = (await _homeExpenses.GetHomeExpense(home.Id, createdExpense!.Id)).Content;
        fetchedExpense.Should().BeEquivalentTo(createdExpense);

        await _homeExpenses.DeleteHomeExpense(home.Id, createdExpense.Id);
    }

    [Fact]
    public async Task Post_invalid_expense_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _homeExpenses.PostHomeExpense(home.Id, new ExpenseProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_expense_creates_expense()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expense = (await _homeExpenses.GetHomeExpenses(home.Id, new FilteredExpensesQuery
        {
            PageNumber = 1,
            PageSize = 10
        })).Content!.Items.First();

        var properties = new ExpenseProperties
        {
            Description = "Bar",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = new[] { "household" },
            IncurredByMemberId = home.Members.First().Id
        };

        var updatedExpenseResponse = await _homeExpenses.PutHomeExpense(home.Id, expense.Id, properties);

        updatedExpenseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedExpense = updatedExpenseResponse.Content;

        updatedExpense.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedExpense = (await _homeExpenses.GetHomeExpense(home.Id, updatedExpense!.Id)).Content;

        fetchedExpense.Should().BeEquivalentTo(updatedExpense);
    }
}
