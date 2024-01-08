using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Web.Store.ExpenseCategories;
using Spenses.Client.Web.Store.Homes;
using Spenses.Shared.Models.ExpenseCategories;
using Spenses.Shared.Models.Homes;

namespace Spenses.Client.Web.Components.ExpenseCategories;

public partial class CreateExpenseCategoryModal
{
    [Inject]
    private IState<ExpenseCategoriesState> ExpenseCategoriesState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private ExpenseCategoryForm ExpenseCategoryFormRef { get; set; } = null!;

    private ExpenseCategoryProperties ExpenseCategory { get; } = new();

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await ExpenseCategoryFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new ExpenseCategoryCreatedAction(Home.Id, ExpenseCategoryFormRef.ExpenseCategory));
    }
}
