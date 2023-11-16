using System.Reactive.Linq;
using Blazorise.DataGrid;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Web.Store.Expenses;
using SortDirection = Spenses.Application.Models.Common.SortDirection;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpensesGrid
{
    [Parameter]
    public Guid HomeId { get; init; }

    [Inject]
    private IState<ExpensesState> ExpensesState { get; init; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    [Inject]
    public IMessageService MessageService { get; init; } = null!;

    private IEnumerable<ExpenseDigest> Expenses => ExpensesState.Value.Expenses.Items;

    private DataGrid<ExpenseDigest> DataGridRef { get; set; } = new();

    private IReadOnlyList<DateOnly?>? FilterDates { get; set; }

    private VirtualizeOptions VirtualizeOptions { get; set; } = new() { DataGridHeight = "750px" };

    private FilteredExpensesQuery Query { get; set; } = new()
    {
        OrderBy = nameof(ExpenseDigest.Date),
        SortDirection = SortDirection.Desc
    };

    protected override void OnInitialized()
    {
        base.OnInitialized();

        //todo: investigate manual read mode
        SubscribeToAction<ExpenseCreationSucceededAction>(async _ =>
        {
            await ModalService.Hide();
            await DataGridRef.Reload();
        });

        SubscribeToAction<ExpenseUpdateSucceededAction>(async _ =>
        {
            await ModalService.Hide();
            await DataGridRef.Reload();
        });

        SubscribeToAction<ExpenseDeletionSucceededAction>(async _ =>
        {
            await DataGridRef.Reload();
        });

        Dispatcher.Dispatch(new ExpenseFiltersRequestedAction(HomeId));
    }

    private Task OnCategoryFilter(IEnumerable<Guid> categoryIds)
    {
        Query.Categories = categoryIds;

        return DataGridRef.Reload();
    }

    private Task OnTagFilter(IEnumerable<string> tags)
    {
        Query.Tags = tags;

        return DataGridRef.Reload();
    }

    private Task OnDatesFilterChanged(IReadOnlyList<DateOnly?> dateValues)
    {
        var dates = dateValues.OrderBy(x => x).ToList();

        Query.MinDate = dates.FirstOrDefault();
        Query.MaxDate = dates.LastOrDefault();

        FilterDates = dateValues;

        return DataGridRef.Reload();
    }

    private Task ClearFilters()
    {
        Query.Tags = null;
        Query.Categories = null;
        Query.MinDate = null;
        Query.MaxDate = null;

        FilterDates = null;

        return DataGridRef.Reload();
    }

    private Task OnDataGridSortChanged(DataGridSortChangedEventArgs args)
    {
        Query.SortDirection = args.SortDirection switch
        {
            Blazorise.SortDirection.Default => null,
            Blazorise.SortDirection.Ascending => SortDirection.Asc,
            Blazorise.SortDirection.Descending => SortDirection.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(args), args.SortDirection, null)
        };

        Query.OrderBy = args.FieldName;

        return DataGridRef.Reload();
    }

    private Task OnDataGridReadData(DataGridReadDataEventArgs<ExpenseDigest> args)
    {
        Query.Skip = args.VirtualizeOffset;
        Query.Take = args.VirtualizeCount;

        Dispatcher.Dispatch(new ExpensesRequestedAction(HomeId, Query));

        return Task.CompletedTask;
    }

    private Task OnExpenseSaved()
    {
        return DataGridRef.Reload();
    }

    private Task OnAddExpenseClicked()
    {
        return ModalService.Show<CreateExpenseModal>();
    }

    private Task OnEditClicked(MouseEventArgs _, Guid expenseId)
    {
        return ModalService.Show<EditExpenseModal>(p =>
        {
            p.Add(x => x.ExpenseId, expenseId);
        });
    }

    private async Task OnDeleteClicked(MouseEventArgs _, ExpenseDigest expense)
    {
        var confirmed = await MessageService.Confirm(
            $"{expense.Amount} paid by {expense.PaidByMemberName} on {expense.Date:O}",
            "Are you sure you want to delete this expense?");

        if (!confirmed)
            return;

        Dispatcher.Dispatch(new ExpenseDeletedAction(HomeId, expense.Id));
    }
}
