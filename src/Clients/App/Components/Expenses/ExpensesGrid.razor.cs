using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Spenses.App.Store.Expenses;
using Spenses.Client.Http;
using Spenses.Shared.Models.Expenses;
using SortDirection = Spenses.Shared.Models.Common.SortDirection;

namespace Spenses.App.Components.Expenses;

public partial class ExpensesGrid
{
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

        SubscribeToAction<ExpenseCreationSucceededAction>(async _ =>
        {
            CreateDialog?.Close();
            await DataGridRef.ReloadServerData();
        });

        SubscribeToAction<ExpenseUpdateSucceededAction>(async _ =>
        {
            EditDialog?.Close();
            await DataGridRef.ReloadServerData();
        });

        SubscribeToAction<ExpenseDeletionSucceededAction>(async _ =>
        {
            await DataGridRef.ReloadServerData();
        });

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

    private FilteredExpensesQuery Query { get; set; } = new()
    {
        OrderBy = nameof(ExpenseDigest.Date),
        SortDirection = SortDirection.Desc
    };

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

    private DateRange? DateRangeValue
    {
        get
        {
            if (!Query.MinDate.HasValue && !Query.MaxDate.HasValue)
                return null;

            var today = DateOnly.FromDateTime(DateTime.Today);

            return new DateRange(
                new DateTime(Query.MinDate.GetValueOrDefault(today), TimeOnly.MinValue),
                new DateTime(Query.MaxDate.GetValueOrDefault(today), TimeOnly.MinValue));
        }
    }

    private Task OnDateFilterChanged(DateRange? range)
    {
        if (range is null)
        {
            Query.MinDate = null;
            Query.MaxDate = null;

            return Task.CompletedTask;
        }

        Query.MinDate = range.Start.HasValue ? DateOnly.FromDateTime(range.Start.GetValueOrDefault()) : null;
        Query.MaxDate = range.End.HasValue ? DateOnly.FromDateTime(range.End.GetValueOrDefault()) : null;

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
