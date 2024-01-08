using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Web.Store.Expenses;
using Spenses.Client.Web.Store.Homes;
using Spenses.Shared.Models.Expenses;
using Spenses.Shared.Models.Homes;

namespace Spenses.Client.Web.Components.Expenses;

public partial class EditExpenseModal
{
    [Parameter]
    public Guid ExpenseId { get; init; }

    [Inject]
    private IState<ExpensesState> ExpensesState { get; init; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; init; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private ExpenseProperties Expense
    {
        get
        {
            var currentExpense = ExpensesState.Value.CurrentExpense;

            if (currentExpense is null)
            {
                return new ExpenseProperties
                {
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    PaidByMemberId = Home.Members.OrderBy(m => m.Name).First().Id,
                    CategoryId = ExpensesState.Value.ExpenseFilters.Categories.First().Id
                };
            }

            return new ExpenseProperties
            {
                Note = currentExpense.Note,
                Date = currentExpense.Date,
                Amount = currentExpense.Amount,
                Tags = currentExpense.Tags,
                PaidByMemberId = currentExpense.PaidByMember.Id,
                CategoryId = currentExpense.Category.Id
            };
        }
    }

    private ExpenseForm ExpenseFormRef { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new ExpenseRequestedAction(Home.Id, ExpenseId));
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await ExpenseFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new ExpenseUpdatedAction(Home.Id, ExpenseId, ExpenseFormRef.Expense));

        await Close();
    }
}
