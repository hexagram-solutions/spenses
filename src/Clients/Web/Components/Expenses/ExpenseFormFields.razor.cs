using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.ExpenseCategories;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Expenses;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpenseFormFields
{
    [Parameter]
    public ExpenseProperties Expense { get; set; } = new();

    private Home Home => GetState<HomeState>().CurrentHome!;

    private ExpensesState ExpensesState => GetState<ExpensesState>();

    private IEnumerable<ExpenseCategory> Categories =>
        ExpensesState.ExpenseFilters?.Categories ?? Enumerable.Empty<ExpenseCategory>();

    private IEnumerable<string> AvailableTags => ExpensesState.ExpenseFilters?.Tags ?? Enumerable.Empty<string>();

    private List<string> ExpenseTags
    {
        get => Expense.Tags.ToList();
        set => Expense.Tags = value.ToArray();
    }
}
