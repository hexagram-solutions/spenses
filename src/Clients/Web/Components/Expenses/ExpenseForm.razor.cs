using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.ExpenseCategories;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Store.ExpenseCategories;
using Spenses.Client.Web.Store.Expenses;
using Spenses.Client.Web.Store.Homes;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpenseForm
{
    [Parameter]
    public ExpenseProperties Expense { get; set; } = new();

    [Inject]
    private IState<ExpensesState> ExpensesState { get; set; } = null!;

    [Inject]
    private IState<ExpenseCategoriesState> ExpenseCategoriesState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = null!;

    public Validations Validations { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private IEnumerable<ExpenseCategory> Categories => ExpenseCategoriesState.Value.ExpenseCategories;

    private IEnumerable<string> AvailableTags => ExpensesState.Value.ExpenseFilters.Tags;

    private List<string> ExpenseTags
    {
        get => Expense.Tags.ToList();
        set => Expense.Tags = [.. value];
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new ExpenseCategoriesRequestedAction(Home.Id));
    }
}
