using System.Runtime.InteropServices.JavaScript;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.App.Store.ExpenseCategories;
using Spenses.App.Store.Expenses;
using Spenses.App.Store.Homes;
using Spenses.Shared.Models.ExpenseCategories;
using Spenses.Shared.Models.Expenses;
using Spenses.Shared.Models.Homes;

namespace Spenses.App.Components.Expenses;

public partial class ExpenseForm
{
    [Parameter]
    [EditorRequired]
    public ExpenseProperties Expense { get; set; } = null!;

    [Inject]
    private IState<ExpensesState> ExpensesState { get; set; } = null!;

    [Inject]
    private IState<ExpenseCategoriesState> ExpenseCategoriesState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private IEnumerable<ExpenseCategory> Categories => ExpenseCategoriesState.Value.ExpenseCategories;

    private IEnumerable<string> AvailableTags => ExpensesState.Value.ExpenseFilters.Tags;

    private DateTime? DateValue
    {
        // Return null if the date isn't set yet. Workaround for making the start month of the date picker work
        // properly.
        get => Expense.Date == DateOnly.MinValue ? null : Expense.Date.ToDateTime(TimeOnly.MinValue);
        set
        {
            if (value.HasValue)
                Expense.Date = DateOnly.FromDateTime(value.GetValueOrDefault());
        }
    }

    private string TagValue { get; set; } = null!;

    private void OnTagValueChanged(string tag)
    {
        if (Expense.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase))
            return;

        Expense.Tags.Add(tag);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new ExpenseCategoriesRequestedAction(Home.Id));
    }

    private Task<IEnumerable<string>> SearchTags(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Task.FromResult(AvailableTags);

        var tags = AvailableTags.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(tags);
    }
}
