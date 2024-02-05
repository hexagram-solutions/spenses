using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Spenses.App.Store.ExpenseCategories;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.App.Components.ExpenseCategories;

public partial class ExpenseCategoriesPanel
{
    [CascadingParameter] public Guid? CurrentHomeId { get; set; }

    [Inject]
    private IState<ExpenseCategoriesState> ExpenseCategoriesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; init; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    private bool IsLoading => ExpenseCategoriesState.Value.ExpenseCategoriesRequesting ||
        ExpenseCategoriesState.Value.ExpenseCategoryCreating ||
        ExpenseCategoriesState.Value.ExpenseCategoryUpdating ||
        ExpenseCategoriesState.Value.ExpenseCategoryDeleting;

    private IEnumerable<ExpenseCategory> ExpenseCategories =>
        ExpenseCategoriesState.Value.ExpenseCategories
            .Where(ec => !ec.IsDefault);

    private IDialogReference? CreateDialog { get; set; }

    private IDialogReference? EditDialog { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new ExpenseCategoriesRequestedAction(CurrentHomeId.GetValueOrDefault()));

        SubscribeToAction<ExpenseCategoryCreatedAction>(_ => CreateDialog?.Close());
        SubscribeToAction<ExpenseCategoryUpdatedAction>(_ => EditDialog?.Close());
    }

    private async Task AddExpenseCategory()
    {
        CreateDialog = await DialogService.ShowAsync<CreateExpenseCategoryDialog>();
    }

    private async Task OnEditClicked(MouseEventArgs _, Guid expenseCategoryId)
    {
        var parameters =
            new DialogParameters<EditExpenseCategoryDialog> { { x => x.ExpenseCategoryId, expenseCategoryId } };

        EditDialog = await DialogService.ShowAsync<EditExpenseCategoryDialog>("Edit expense category", parameters);
    }

    private void OnDeleteClicked()
    {
        Snackbar.Add("That feature is not implemented yet.", Severity.Error);
    }
}
