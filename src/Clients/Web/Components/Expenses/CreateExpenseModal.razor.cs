using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Store.Expenses;
using Spenses.Client.Web.Store.Homes;

namespace Spenses.Client.Web.Components.Expenses;

public partial class CreateExpenseModal
{
    [Inject]
    private IState<ExpensesState> ExpensesState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    public ExpenseProperties Expense { get; set; } = new();

    private ExpenseForm ExpenseFormRef { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Expense = new ExpenseProperties
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            PaidByMemberId = Home.Members.OrderBy(m => m.Name).First().Id,
            CategoryId = ExpensesState.Value.ExpenseFilters.Categories.First().Id
        };
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await ExpenseFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new ExpenseCreatedAction(Home.Id, ExpenseFormRef.Expense));

        await Close();
    }
}
