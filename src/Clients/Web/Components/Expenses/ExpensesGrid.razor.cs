using System.Collections.ObjectModel;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Web.Features.Expenses;
using SortDirection = Spenses.Application.Models.Common.SortDirection;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpensesGrid : BlazorState.BlazorStateComponent
{

    private DataGrid<ExpenseDigest> DataGridRef { get; set; } = new();

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

        Query.Categories = ExpensesState.ExpenseFilters?.Categories.Select(c => c.Id);

        await base.OnParametersSetAsync();
    }

    private Task OnCategoryFilter(IEnumerable<Guid> categoryIds)
    {
        Query.Categories = categoryIds.ToArray();

        return DataGridRef.Reload();
    }

    private bool CustomFilterHandler(ExpenseDigest expense)
    {
        return true;
        //var hasSearchValue = !string.IsNullOrEmpty(searchValue);
        //var hasSearchCategoryValue = !string.IsNullOrEmpty(searchCategoryValue);

        //if (!hasSearchValue && !hasSearchCategoryValue)
        //    return true;

        //var valid = true;
        //if (hasSearchValue)
        //    valid = expense.Name?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true
        //        || expense.Description?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true
        //        || expense.Price?.Contains(searchValue, StringComparison.OrdinalIgnoreCase) == true;

        //if (hasSearchCategoryValue)
        //    valid = valid & expense.Category == searchCategoryValue;

        //return valid;
    }
    private async Task OnDataGridReadData(DataGridReadDataEventArgs<ExpenseDigest> args)
    {
        Query.Skip = args.VirtualizeOffset;
        Query.Take = args.VirtualizeCount;

        await Mediator.Send(new ExpensesState.ExpensesRequested(HomeId, Query));
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
