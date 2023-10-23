using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;

namespace Spenses.Api.IntegrationTests.Expenses;

public partial class ExpensesIntegrationTests
{
    [Fact]
    public async Task Get_filters_yields_filterable_values()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Skip = 0,
            Take = 25
        })).Content!.Items;

        var distinctTags = expenses
            .SelectMany(t => t.Tags?.Split(' ') ?? Array.Empty<string>())
            .Distinct();

        var categories = (await _expenseCategories.GetExpenseCategories(home.Id)).Content!;

        var filterValues = (await _expenses.GetExpenseFilters(home.Id)).Content!;

        filterValues.Tags.Should().BeEquivalentTo(distinctTags);
        filterValues.Tags.Should().BeInAscendingOrder();

        filterValues.Categories.Should().BeEquivalentTo(categories.ToDictionary(k => k.Id, v => v.Name));
        filterValues.Categories.Values.Should().BeInAscendingOrder();
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
            PaidByMemberId = home.Members.First().Id
        })).Content!;

        var expenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Skip = 0,
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
    public async Task Get_expenses_with_period_filters_yields_expenses_in_range()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var unfilteredExpenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Skip = 0,
            Take = 100
        })).Content!.Items.ToList();

        var earliestExpenseDate = unfilteredExpenses.MinBy(x => x.Date)!.Date;
        var latestExpenseDate = unfilteredExpenses.MaxBy(x => x.Date)!.Date;

        var minDateFilterValue = earliestExpenseDate.AddDays(1);
        var maxDateFilterValue = latestExpenseDate.AddDays(-1);

        var filteredExpenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Skip = 0,
            Take = 100,
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
    public async Task Get_expenses_with_category_filters_yields_expenses_in_category()
    {
        var home = (await _homes.GetHomes()).Content!.First();

        var expenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Skip = 0,
            Take = 100
        })).Content!.Items;

        var expense = expenses.First(e => e.CategoryId != null);

        var filteredExpenses = (await _expenses.GetExpenses(home.Id, new FilteredExpensesQuery
        {
            Skip = 0,
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

        var query = new FilteredExpensesQuery
        {
            Skip = 0,
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
}
