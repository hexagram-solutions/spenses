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

    public ExpenseProperties Expense { get; set; } = new()
    {
        Date = DateOnly.FromDateTime(DateTime.Today)
    };

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Validations Validations { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private ExpensesState ExpensesState => GetState<ExpensesState>();

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Create()
    {
        if (!await Validations.ValidateAll())
            return;

        //todo: if we created a new category, delete it if it's not been selected

        await Mediator.Send(new ExpensesState.ExpenseCreated(Home.Id, Expense));

        await OnSave();

        await Close();
    }
}
