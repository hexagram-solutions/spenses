using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpenseShareRow
{

    [Parameter]
    public ExpenseShareProperties ExpenseShare { get; set; } = null!;

    [Parameter]
    public ExpenseProperties Expense { get; set; } = null!;

    [Parameter]
    public Member Member { get; set; } = null!;

    private decimal OwedAmount
    {
        get => ExpenseShare.OwedAmount;
        set
        {
            ExpenseShare.OwedAmount = value;

            _owedPercentage = Expense.Amount > 0 ? ExpenseShare.OwedAmount / Expense.Amount : 0.00m;
        }
    }

    private decimal OwedPercentage
    {
        get => _owedPercentage * 100;
        set
        {
            _owedPercentage = value / 100m;

            OwedAmount = Expense.Amount * _owedPercentage;
        }
    }

    private decimal _owedPercentage;
}
