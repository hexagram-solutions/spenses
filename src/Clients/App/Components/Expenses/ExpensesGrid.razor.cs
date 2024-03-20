using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Spenses.App.Components.Shared;
using Spenses.App.Store.Expenses;
using Spenses.Client.Http;
using Spenses.Shared.Models.Expenses;
using SortDirection = Spenses.Shared.Models.Common.SortDirection;

namespace Spenses.App.Components.Expenses;

public partial class ExpensesGrid
{
    public ExpensesGrid()
    {
        var today = DateTime.Today;

        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

        Query = new FilteredExpensesQuery
        {
            OrderBy = nameof(ExpenseDigest.Date),
            SortDirection = SortDirection.Desc,
            MinDate = new DateOnly(today.Year, today.Month, 1),
            MaxDate = new DateOnly(today.Year, today.Month, daysInMonth)
        };
    }

    [Parameter]
    public Guid HomeId { get; init; }

    [Inject]
    private IState<ExpensesState> ExpensesState { get; init; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = null!;

    [Inject]
    private IDialogService DialogService { get; init; } = null!;

    [Inject]
    private IExpensesApi ExpensesApi { get; init; } = null!;

    private MudDataGrid<ExpenseDigest> DataGridRef { get; set; } = new();

    private IDialogReference? CreateDialog { get; set; }

    private IDialogReference? EditDialog { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.SubscribeToAsyncAction<ExpenseCreationSucceededAction>(_ =>
        {
            CreateDialog?.Close();
            return DataGridRef.ReloadServerData();
        });

        this.SubscribeToAsyncAction<ExpenseUpdateSucceededAction>(_ =>
        {
            EditDialog?.Close();
            return DataGridRef.ReloadServerData();
        });

        this.SubscribeToAsyncAction<ExpenseDeletionSucceededAction>(_ => DataGridRef.ReloadServerData());

        Dispatcher.Dispatch(new ExpenseFiltersRequestedAction(HomeId));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await DataGridRef.SetSortAsync(nameof(ExpenseDigest.Date), MudBlazor.SortDirection.Descending, x => x.Date);
    }

    private async Task CreateExpense()
    {
        CreateDialog = await DialogService.ShowAsync<CreateExpenseDialog>();
    }

    private async Task OnEditClicked(MouseEventArgs _, Guid expenseId)
    {
        var parameters =
            new DialogParameters<EditExpenseDialog> { { x => x.ExpenseId, expenseId } };

        EditDialog = await DialogService.ShowAsync<EditExpenseDialog>("Edit member", parameters);
    }

    private async Task OnDeleteClicked(MouseEventArgs _, ExpenseDigest expense)
    {
        var confirmed = await DialogService.ShowMessageBox(
            "Are you sure you want to delete this expense?",
            $"{expense.Amount} paid by {expense.PaidByMemberName} on {expense.Date:O}",
            "Delete expense",
            cancelText: "Close");

        if (confirmed != true)
            return;

        Dispatcher.Dispatch(new ExpenseDeletedAction(HomeId, expense.Id));
    }

    private FilteredExpensesQuery Query { get; }

    private Task OnCategoryFilter(IEnumerable<Guid> categoryIds)
    {
        Query.Categories = categoryIds;

        return DataGridRef.ReloadServerData();
    }

    private Task OnTagFilter(IEnumerable<string> tags)
    {
        Query.Tags = tags;

        return DataGridRef.ReloadServerData();
    }

    private object GroupByDate(ExpenseDigest expenseDigest)
    {
        return new DateOnly(expenseDigest.Date.Year, expenseDigest.Date.Month, 1);
    }

    private DateRange Period => new(
        new DateTime(Query.MinDate, TimeOnly.MinValue),
        new DateTime(Query.MaxDate, TimeOnly.MinValue));

    private Task OnPeriodFilterChanged(DateRange range)
    {
        Query.MinDate = DateOnly.FromDateTime(range.Start.GetValueOrDefault());
        Query.MaxDate = DateOnly.FromDateTime(range.End.GetValueOrDefault());

        return DataGridRef.ReloadServerData();
    }

    private async Task<GridData<ExpenseDigest>> GetServerData(GridState<ExpenseDigest> state)
    {
        var sortDefinition = state.SortDefinitions.SingleOrDefault();

        if (sortDefinition is not null)
        {
            Query.SortDirection = sortDefinition.Descending ? SortDirection.Desc : SortDirection.Asc;
            Query.OrderBy = sortDefinition.SortBy;
        }

        var response = await ExpensesApi.GetExpenses(HomeId, Query);

        return new GridData<ExpenseDigest>
        {
            Items = response.Content!.Items,
            TotalItems = response.Content.TotalCount
        };
    }
}
