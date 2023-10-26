using BlazorState;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Expenses;

namespace Spenses.Client.Web.Features.Expenses;

public partial class ExpensesState : State<ExpensesState>
{
    public PagedResult<ExpenseDigest>? Expenses { get; private set; }

    public bool ExpensesRequesting { get; private set; }

    public ExpenseFilters? ExpenseFilters { get; private set; }

    public bool ExpenseFiltersRequesting { get; private set; }

    public override void Initialize()
    {
        Expenses = new PagedResult<ExpenseDigest>(0, Array.Empty<ExpenseDigest>());
        ExpenseFilters = new ExpenseFilters();
    }
}