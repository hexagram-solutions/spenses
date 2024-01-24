using Fluxor;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Expenses;

namespace Spenses.App.Store.Expenses;

[FeatureState(Name = "Expenses", CreateInitialStateMethodName = nameof(Initialize))]
public record ExpensesState
{
    private static ExpensesState Initialize()
    {
        return new ExpensesState();
    }

    public Expense? CurrentExpense { get; init; }

    public PagedResult<ExpenseDigest> Expenses { get; init; } = new(0, []);

    public ExpenseFilters ExpenseFilters { get; init; } = new();

    public bool ExpensesRequesting { get; init; }

    public bool ExpenseRequesting { get; init; }

    public bool ExpenseCreating { get; init; }

    public bool ExpenseUpdating { get; init; }

    public bool ExpenseDeleting { get; init; }

    public bool ExpenseFiltersRequesting { get; init; }
}
