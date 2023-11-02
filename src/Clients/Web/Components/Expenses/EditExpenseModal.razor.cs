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

    private Validations Validations { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private ExpensesState ExpensesState => GetState<ExpensesState>();

    private ExpenseProperties Expense { get; set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        await Mediator.Send(new ExpensesState.ExpenseSelected(Home.Id, ExpenseId));

        var currentExpense = ExpensesState.CurrentExpense!;

        Expense = new ExpenseProperties
        {
            Amount = currentExpense.Amount,
            Date = currentExpense.Date,
            Description = currentExpense.Description,
            CategoryId = currentExpense.Category?.Id,
            PaidByMemberId = currentExpense.PaidByMember.Id,
            Tags = currentExpense.Tags
        };

        await base.OnParametersSetAsync();
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await Validations.ValidateAll())
            return;

        await Mediator.Send(new ExpensesState.ExpenseUpdated(Home.Id, ExpenseId, Expense));

        await Close();

        await OnSave();
    }
}
