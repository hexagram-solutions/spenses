using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Expenses;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Expenses;

public partial class EditExpenseModal
{
    [Parameter]
    public Guid ExpenseId { get; set; }

    [Parameter]
    public Func<Task> OnSave { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private ExpensesState ExpensesState => GetState<ExpensesState>();

    private ExpenseProperties Expense { get; set; } = new();

    private ExpenseForm ExpenseFormRef { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await Mediator.Send(new ExpensesState.ExpenseSelected(Home.Id, ExpenseId));

        // Direct mapping to new object to ensure correct type is passed to validator
        var currentExpense = ExpensesState.CurrentExpense!;

        Expense = new ExpenseProperties
        {
            Amount = currentExpense.Amount,
            Date = currentExpense.Date,
            Note = currentExpense.Note,
            CategoryId = currentExpense.Category?.Id,
            PaidByMemberId = currentExpense.PaidByMember.Id,
            Tags = currentExpense.Tags
        };

        await base.OnInitializedAsync();
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await ExpenseFormRef.Validations.ValidateAll())
            return;

        await Mediator.Send(new ExpensesState.ExpenseUpdated(Home.Id, ExpenseId, Expense));

        await Close();

        await OnSave();
    }
}
