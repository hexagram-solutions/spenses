using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        var home = (await _homes.GetHomes()).First();

        var properties = new ExpenseProperties
        {
            Description = "Foo",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            IncurredByMemberId = home.Members.First().Id
        };

        var createdExpense = await _homeExpenses.PostHomeExpense(home.Id, properties);
        createdExpense.Should().BeEquivalentTo(properties, opts => opts.Excluding(x => x.IncurredByMemberId));

        var fetchedExpense = await _homeExpenses.GetHomeExpense(home.Id, createdExpense.Id);
        fetchedExpense.Should().BeEquivalentTo(createdExpense);

        var expenses = await _homeExpenses.GetHomeExpenses(home.Id);
        expenses.Should().ContainEquivalentOf(createdExpense);

        await _homeExpenses.DeleteHomeExpense(home.Id, createdExpense.Id);
    }

    [Fact]
    public async Task Put_expense_creates_expense()
    {
        var home = (await _homes.GetHomes()).First();

        var expense = (await _homeExpenses.GetHomeExpenses(home.Id)).First();

        var properties = new ExpenseProperties
        {
            Description = "Foo",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            IncurredByMemberId = home.Members.First().Id
        };

        var updatedExpense = await _homeExpenses.PutHomeExpense(home.Id, expense.Id, properties);
        updatedExpense.Should().BeEquivalentTo(properties, opts => opts.Excluding(x => x.IncurredByMemberId));

        var fetchedExpense = await _homeExpenses.GetHomeExpense(home.Id, updatedExpense.Id);
        fetchedExpense.Should().BeEquivalentTo(updatedExpense);
    }
}
