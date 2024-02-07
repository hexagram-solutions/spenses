using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Store.ExpenseCategories;
using Spenses.App.Store.Homes;
using Spenses.Shared.Models.ExpenseCategories;
using Spenses.Shared.Models.Homes;

namespace Spenses.App.Components.ExpenseCategories;

public partial class CreateExpenseCategoryDialog
{
    [CascadingParameter]
    private MudDialogInstance Dialog { get; set; } = null!;

    [Inject]
    private IState<ExpenseCategoriesState> ExpenseCategoriesState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    public ExpenseCategoryProperties ExpenseCategory { get; set; } = new();

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save()
    {
        Dispatcher.Dispatch(new ExpenseCategoryCreatedAction(Home.Id, ExpenseCategory));
    }
}
