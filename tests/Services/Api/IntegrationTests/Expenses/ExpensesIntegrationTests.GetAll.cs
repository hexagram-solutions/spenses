using System.Net;
using Hexagrams.Extensions.Common;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests
{
    [Fact]
    public async Task Get_expenses_with_invalid_identifiers_yields_not_found()
    {
        var homeNotFoundResult = await _expenses.GetExpenses(Guid.NewGuid(), DefaultExpensesQuery);

        homeNotFoundResult.Error!.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_filters_yields_filterable_values()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expenses = (await _expenses.GetExpenses(home.Id, DefaultExpensesQuery with
        {
            Take = 25
        })).Content!.Items;

        var distinctTags = expenses
            .SelectMany(t => t.Tags?.Split(' ') ?? [])
            .Distinct();

        var categories = (await _expenseCategories.GetExpenseCategories(home.Id)).Content!;

        var filterValues = (await _expenses.GetExpenseFilters(home.Id)).Content!;

        filterValues.Tags.Should().BeEquivalentTo(distinctTags);
        filterValues.Tags.Should().BeInAscendingOrder();

        filterValues.Categories.Should().BeEquivalentTo(categories);
        filterValues.Categories.Should()
            .BeInDescendingOrder(cat => cat.IsDefault)
            .And.ThenBeInAscendingOrder(cat => cat.Name);
    }

    [Fact]
    public async Task Get_expenses_with_tag_filters_yields_tagged_expenses()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var category = (await _expenseCategories.GetExpenseCategories(home.Id)).Content!.First();

        var tags = new[] { "bills", "groceries" };

        var expense = (await _expenses.PostExpense(home.Id, new ExpenseProperties
        {
            Note = "Foo",
            Amount = 1234.56m,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Tags = [.. tags],
            CategoryId = category.Id,
            PaidByMemberId = home.Members.First().Id
        })).Content!;

        var expenses = (await _expenses.GetExpenses(home.Id, DefaultExpensesQuery with
        {
            Take = 25,
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
    public async Task Get_expenses_with_date_filters_yields_expenses_in_range()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var query = DefaultExpensesQuery;

        var filteredExpenses = (await _expenses.GetExpenses(home.Id, query)).Content!.Items;

        filteredExpenses.Should().AllSatisfy(e =>
        {
            e.Date.Should()
                .BeOnOrAfter(query.MinDate)
                .And.BeOnOrBefore(query.MaxDate);
        });
    }

    [Fact]
    public async Task Get_expenses_with_category_filters_yields_expenses_in_category()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expenses = (await _expenses.GetExpenses(home.Id, DefaultExpensesQuery with
        {
            Take = 100
        })).Content!.Items;

        var expense = expenses.First(e => e.CategoryId != null);

        var filteredExpenses = (await _expenses.GetExpenses(home.Id, DefaultExpensesQuery with
        {
            Take = 100,
            Categories = new[] { expense.CategoryId.GetValueOrDefault() }
        })).Content!.Items.ToList();

        filteredExpenses.Should().AllSatisfy(e =>
        {
            e.CategoryId.Should().Be(expense.CategoryId);
            e.CategoryName.Should().Be(expense.CategoryName);
        });
    }

    [Fact]
    public async Task Get_expenses_ordered_by_amount_yields_ordered_expenses()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var query = DefaultExpensesQuery with
        {
            Take = 25,
            OrderBy = nameof(ExpenseDigest.Amount),
            SortDirection = SortDirection.Asc
        };

        var expenses = (await _expenses.GetExpenses(home.Id, query)).Content!.Items;

        expenses.Should().BeInAscendingOrder(x => x.Amount);

        expenses = (await _expenses.GetExpenses(home.Id, query with { SortDirection = SortDirection.Desc }))
            .Content!.Items;

        expenses.Should().BeInDescendingOrder(x => x.Amount);
    }

    [Fact]
    public async Task Get_expenses_with_complex_query_yields_expected_expenses()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var filters = (await _expenses.GetExpenseFilters(home.Id)).Content!;

        var query = DefaultExpensesQuery with
        {
            Take = 25,
            OrderBy = nameof(ExpenseDigest.Date),
            SortDirection = SortDirection.Desc,
            Tags = filters.Tags.First().Yield(),
            Categories = filters.Categories.First().Id.Yield()
        };

        var expenses = (await _expenses.GetExpenses(home.Id, query)).Content!.Items.ToList();

        expenses.Should().BeInDescendingOrder(x => x.Date);

        expenses.Should().AllSatisfy(x =>
        {
            x.Tags.Should().Contain(query.Tags.Single());
            x.CategoryId.Should().Be(query.Categories.Single());
        });
    }
}
