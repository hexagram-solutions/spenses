using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Expenses;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Expenses;

public partial class CreateExpenseModal
{
    [Parameter]
    public Func<Task> OnSave { get; set; } = null!;

    public ExpenseProperties Expense { get; set; } = new();

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private ExpenseForm ExpensesFormRef { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private ExpensesState ExpensesState => GetState<ExpensesState>();

    protected override Task OnInitializedAsync()
    {
        Expense = new ExpenseProperties
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            PaidByMemberId = Home.Members.OrderBy(m => m.Name).First().Id
        };

        return base.OnInitializedAsync();
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await ExpensesFormRef.Validations.ValidateAll())
            return;

        await Mediator.Send(new ExpensesState.ExpenseCreated(Home.Id, Expense));

        await Close();

        await OnSave();
    }
}
