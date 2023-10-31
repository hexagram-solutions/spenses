using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Web.Features.Expenses;
using SortDirection = Spenses.Application.Models.Common.SortDirection;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpensesGrid : BlazorState.BlazorStateComponent
{
    private DataGrid<ExpenseDigest> DataGridRef { get; set; } = new();

    private IReadOnlyList<DateOnly?>? FilterDates { get; set; }

    private VirtualizeOptions VirtualizeOptions { get; set; } = new() { DataGridHeight = "750px" };

    [Parameter]
    public Guid HomeId { get; set; }

    private ExpensesState ExpensesState => GetState<ExpensesState>();

    private FilteredExpensesQuery Query { get; set; } = new()
    {
        Skip = 0,
        Take = 25,
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

    private Task OnMinDateFilter(DateOnly? date)
    {
        Query.MinDate = date;

        return DataGridRef.Reload();
    }

    private Task OnMaxDateFilter(DateOnly? date)
    {
        Query.MaxDate = date;

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

    private Task ToEdit()
    {
        return Task.CompletedTask;
    }

    private Task ToDelete()
    {
        return Task.CompletedTask;
    }
}
