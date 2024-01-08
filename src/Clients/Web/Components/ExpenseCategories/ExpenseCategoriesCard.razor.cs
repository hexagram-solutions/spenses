using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spenses.Client.Web.Store.ExpenseCategories;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Client.Web.Components.ExpenseCategories;

public partial class ExpenseCategoriesCard
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    private IState<ExpenseCategoriesState> ExpenseCategoriesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    [Inject]
    public IMessageService MessageService { get; set; } = null!;

    private bool IsLoading => ExpenseCategoriesState.Value.ExpenseCategoriesRequesting ||
        ExpenseCategoriesState.Value.ExpenseCategoryCreating ||
        ExpenseCategoriesState.Value.ExpenseCategoryUpdating ||
        ExpenseCategoriesState.Value.ExpenseCategoryDeleting;

    private IEnumerable<ExpenseCategory> ExpenseCategories =>
        ExpenseCategoriesState.Value.ExpenseCategories
            .Where(ec => !ec.IsDefault);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<ExpenseCategoryCreationSucceededAction>(async _ =>
        {
            await ModalService.Hide();
        });

        SubscribeToAction<ExpenseCategoryUpdateSucceededAction>(async _ =>
        {
            await ModalService.Hide();
        });

        Dispatcher.Dispatch(new ExpenseCategoriesRequestedAction(HomeId));
    }

    private Task AddExpenseCategory()
    {
        return ModalService.Show<CreateExpenseCategoryModal>();
    }

    private Task OnEditClicked(MouseEventArgs _, Guid expenseCategoryId)
    {
        return ModalService.Show<EditExpenseCategoryModal>(p =>
        {
            p.Add(x => x.ExpenseCategoryId, expenseCategoryId);
        });
    }

    private async Task OnDeleteClicked(MouseEventArgs _, ExpenseCategory expenseCategory)
    {
        await MessageService.Error("You can't do that yet.", "Not implemented");
    }
}
