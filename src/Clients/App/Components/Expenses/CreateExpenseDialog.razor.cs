using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using MudBlazor;
using Spenses.App.Store.Expenses;
using Spenses.App.Store.Homes;
using Spenses.Shared.Models.Expenses;
using Spenses.Shared.Models.Homes;

namespace Spenses.App.Components.Expenses;

public partial class CreateExpenseDialog
{
    [CascadingParameter]
    private MudDialogInstance Dialog { get; set; } = null!;

    [Inject]
    private IState<ExpensesState> ExpensesState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;
    public ExpenseProperties Expense { get; set; } = new();

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new ExpenseCreatedAction(Home.Id, Expense));
    }
}
