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

        _homeExpenses = RestService.For<IHomeExpensesApi>(fixture.WebApplicationFactory.CreateClient(),
            new RefitSettings { CollectionFormat = CollectionFormat.Multi });
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
            Tags = new[] { "groceries" },
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

    [Fact]
    public async Task Get_filters_yields_filterable_values()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expenses = (await _homeExpenses.GetHomeExpenses(home.Id, new FilteredExpensesQuery
        {
            PageNumber = 1,
            PageSize = 25
        })).Content!.Items;

        var distinctTags = expenses
            .SelectMany(t => t.Tags?.Split(' ') ?? Array.Empty<string>())
            .Distinct();

        var filterValues = (await _homeExpenses.GetExpenseFilters(home.Id)).Content!;

        filterValues.Tags.Should().BeEquivalentTo(distinctTags);
        filterValues.Tags.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Get_expenses_with_tag_filters_yields_tagged_expenses()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var tags = new[] { "bills", "groceries" };

        var expense = (await _homeExpenses.PostHomeExpense(home.Id, new ExpenseProperties
        {
            Description = "Foo",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = tags,
            IncurredByMemberId = home.Members.First().Id
        })).Content!;

        var expenses = (await _homeExpenses.GetHomeExpenses(home.Id, new FilteredExpensesQuery
        {
            PageNumber = 1,
            PageSize = 25,
            Tags = tags
        })).Content!.Items;

        expenses.Should().AllSatisfy(e =>
        {
            var expenseTags = e.Tags?.Split(' ');

            expenseTags.Should().BeEquivalentTo(tags);
        });

        await _homeExpenses.DeleteHomeExpense(home.Id, expense.Id);
    }
}
