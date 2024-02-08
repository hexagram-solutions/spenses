using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Store.Expenses;
using Spenses.App.Store.Homes;
using Spenses.Shared.Models.Expenses;
using Spenses.Shared.Models.Homes;

namespace Spenses.App.Components.Expenses;

public partial class EditExpenseDialog
{
    [Parameter]
    [EditorRequired]
    public Guid ExpenseId { get; set; }

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

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<ExpenseReceivedAction>(a => Expense = new ExpenseProperties
        {
            Amount = a.Expense.Amount,
            CategoryId = a.Expense.Category.Id,
            Date = a.Expense.Date,
            Note = a.Expense.Note,
            PaidByMemberId = a.Expense.PaidByMember.Id,
            Tags = a.Expense.Tags
        });

        Dispatcher.Dispatch(new ExpenseRequestedAction(Home.Id, ExpenseId));
    }

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save()
    {
        Dispatcher.Dispatch(new ExpenseUpdatedAction(Home.Id, ExpenseId, Expense));
    }
}
