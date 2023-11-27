using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Client.Web.Store.Homes;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpenseSplittingOptions
{
    [Parameter]
    public ExpenseProperties Expense { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; init; } = null!;

    private List<ExpenseShareRowModel> RowModels { get; set; } = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();

        RowModels = Expense.ExpenseShares
            .Select(es => new ExpenseShareRowModel(es,
                HomesState.Value.CurrentHome!.Members.Single(m => m.Id == es.OwedByMemberId), Expense))
            .ToList();
    }

    public void UpdateExpenseShareAmounts()
    {
        foreach (var row in RowModels)
        {
            row.OwedAmount = Expense.Amount * (row.OwedPercentage / 100);
        }
    }
}
