using Fluxor;
using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;

namespace Spenses.Client.Web.Store.Expenses;

[FeatureState(Name = "Expenses", CreateInitialStateMethodName = nameof(Initialize))]
public record ExpensesState
{
    private static ExpensesState Initialize()
    {
        return new ExpensesState();
    }

    public Expense? CurrentExpense { get; init; }

    public PagedResult<ExpenseDigest> Expenses { get; init; } = new(0, Enumerable.Empty<ExpenseDigest>());

    public ExpenseFilters ExpenseFilters { get; init; } = new();

    public bool ExpensesRequesting { get; init; }

    public bool ExpenseRequesting { get; init; }

    public bool ExpenseCreating { get; init; }

    public bool ExpenseUpdating { get; init; }

    public bool ExpenseDeleting { get; init; }

    public bool ExpenseFiltersRequesting { get; init; }

    public ApiException? Error { get; init; }
}
