using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Expenses;
using Spenses.Client.Web.Features.Homes;
using SortDirection = Spenses.Application.Models.Common.SortDirection;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpensesGrid : BlazorState.BlazorStateComponent
{
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    [Inject]
    public IMessageService MessageService { get; set; } = null!;

    private DataGrid<ExpenseDigest> DataGridRef { get; set; } = new();

    private IReadOnlyList<DateOnly?>? FilterDates { get; set; }

    private VirtualizeOptions VirtualizeOptions { get; set; } = new() { DataGridHeight = "750px" };

    [Parameter]
    public Guid HomeId { get; set; }

    private ExpensesState ExpensesState => GetState<ExpensesState>();

    private FilteredExpensesQuery Query { get; set; } = new()
    {
        OrderBy = nameof(ExpenseDigest.Date),
        SortDirection = SortDirection.Desc
    };

    protected override async Task OnParametersSetAsync()
    {
        await Mediator.Send(new ExpensesState.ExpenseFiltersRequested(HomeId));

        await base.OnParametersSetAsync();
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

        return Mediator.Send(new ExpensesState.ExpensesRequested(HomeId, Query), args.CancellationToken);
    }

    private Task OnExpenseSaved()
    {
        return DataGridRef.Reload();
    }

    private Task OnAddExpenseClicked()
    {
        return ModalService.Show<CreateExpenseModal>(p => p.Add(x => x.OnSave, OnExpenseSaved));
    }

    private Task OnEditClicked(MouseEventArgs _, Guid expenseId)
    {
        return ModalService.Show<EditExpenseModal>(p =>
        {
            p.Add(x => x.ExpenseId, expenseId);
            p.Add(x => x.OnSave, OnExpenseSaved);
        });
    }

    private async Task OnDeleteClicked(MouseEventArgs _, ExpenseDigest expense)
    {
        var confirmed = await MessageService.Confirm($"{expense.Amount} paid by {expense.PaidByMemberName} on {expense.Date:O}",
            "Are you sure you want to delete this expense?");

        if (!confirmed)
            return;

        await Mediator.Send(new ExpensesState.ExpenseDeleted(HomeId, expense.Id));

        await DataGridRef.Reload();
    }
}
