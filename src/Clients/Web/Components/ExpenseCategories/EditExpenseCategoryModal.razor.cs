using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Web.Store.ExpenseCategories;
using Spenses.Client.Web.Store.Homes;
using Spenses.Shared.Models.ExpenseCategories;
using Spenses.Shared.Models.Homes;

namespace Spenses.Client.Web.Components.ExpenseCategories;

public partial class EditExpenseCategoryModal
{
    [Parameter]
    public Guid ExpenseCategoryId { get; set; }

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

    private ExpenseCategoryProperties ExpenseCategory
    {
        get
        {
            var currentCategory = ExpenseCategoriesState.Value.CurrentExpenseCategory;

            if (currentCategory is null)
                return new ExpenseCategoryProperties();

            return new ExpenseCategoryProperties
            {
                Name = currentCategory.Name,
                Description = currentCategory.Description
            };
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new ExpenseCategoryRequestedAction(Home.Id, ExpenseCategoryId));
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await ExpenseCategoryFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new ExpenseCategoryUpdatedAction(Home.Id, ExpenseCategoryId,
            ExpenseCategoryFormRef.ExpenseCategory));
    }
}
