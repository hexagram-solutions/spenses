using System.Net;
using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Http;

namespace Spenses.Api.IntegrationTests.Controllers;

[Collection(WebApplicationCollection.CollectionName)]
public class ExpensesIntegrationTests
{
    private readonly IHomesApi _homes;
    private readonly IExpensesApi _expenses;

    public ExpensesIntegrationTests(WebApplicationFixture<Program> fixture)
    {
        _homes = RestService.For<IHomesApi>(fixture.WebApplicationFactory.CreateClient());

        _expenses = RestService.For<IExpensesApi>(fixture.WebApplicationFactory.CreateClient(),
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

        var createdExpenseResponse = await _expenses.PostExpense(home.Id, properties);

        createdExpenseResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdExpense = createdExpenseResponse.Content;

        createdExpense.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedExpense = (await _expenses.GetExpense(home.Id, createdExpense!.Id)).Content;
        fetchedExpense.Should().BeEquivalentTo(createdExpense);

        await _expenses.DeleteExpense(home.Id, createdExpense.Id);
    }

    [Fact]
    public async Task Post_invalid_expense_yields_bad_request()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var result = await _expenses.PostExpense(home.Id, new ExpenseProperties());

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Put_expense_creates_expense()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expense = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
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

        var updatedExpenseResponse = await _expenses.PutExpense(home.Id, expense.Id, properties);

        updatedExpenseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedExpense = updatedExpenseResponse.Content;

        updatedExpense.Should().BeEquivalentTo(properties, opts =>
            opts.ExcludingNestedObjects()
                .ExcludingMissingMembers());

        var fetchedExpense = (await _expenses.GetExpense(home.Id, updatedExpense!.Id)).Content;

        fetchedExpense.Should().BeEquivalentTo(updatedExpense);
    }

    [Fact]
    public async Task Get_filters_yields_filterable_values()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            PageNumber = 1,
            PageSize = 25
        })).Content!.Items;

        var distinctTags = expenses
            .SelectMany(t => t.Tags?.Split(' ') ?? Array.Empty<string>())
            .Distinct();

        var filterValues = (await _expenses.GetExpenseFilters(home.Id)).Content!;

        filterValues.Tags.Should().BeEquivalentTo(distinctTags);
        filterValues.Tags.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task Get_expenses_with_tag_filters_yields_tagged_expenses()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var tags = new[] { "bills", "groceries" };

        var expense = (await _expenses.PostExpense(home.Id, new ExpenseProperties
        {
            Description = "Foo",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = tags,
            IncurredByMemberId = home.Members.First().Id
        })).Content!;

        var expenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
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

        await _expenses.DeleteExpense(home.Id, expense.Id);
    }

    [Fact]
    public async Task Get_expenses_with_period_filters_yields_expenses_in_range()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var unfilteredExpenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            PageNumber = 1,
            PageSize = 100
        })).Content!.Items.ToList();

        var earliestExpenseDate = unfilteredExpenses.MinBy(x => x.Date)!.Date;
        var latestExpenseDate = unfilteredExpenses.MaxBy(x => x.Date)!.Date;

        var minDateFilterValue = earliestExpenseDate.AddDays(1);
        var maxDateFilterValue = latestExpenseDate.AddDays(-1);

        var filteredExpenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            PageNumber = 1,
            PageSize = 100,
            MinDate = minDateFilterValue,
            MaxDate = maxDateFilterValue
        })).Content!.Items;

        filteredExpenses.Should().AllSatisfy(e =>
        {
            e.Date.Should().BeOnOrAfter(minDateFilterValue)
                .And.BeOnOrBefore(maxDateFilterValue);
        });
    }

    [Fact]
    public async Task Get_expenses_ordered_by_amount_yields_ordered_expenses()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var query = new FilteredExpensesQuery
        {
            PageNumber = 1,
            PageSize = 25,
            OrderBy = nameof(ExpenseDigest.Amount),
            SortDirection = SortDirection.Asc
        };

        var expenses = (await _expenses.GetExpenses(home.Id, query)).Content!.Items;

        expenses.Should().BeInAscendingOrder(x => x.Amount);

        expenses = (await _expenses.GetExpenses(home.Id, query with { SortDirection = SortDirection.Desc }))
            .Content!.Items;

        expenses.Should().BeInDescendingOrder(x => x.Amount);
    }
}
