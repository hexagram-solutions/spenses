using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Web.Components.Expenses;

public partial class ExpenseShareRow
{
    [Parameter]
    public ExpenseShareRowModel Model { get; set; } = null!;
}

public class ExpenseShareRowModel
{
    private readonly ExpenseShareProperties _expenseShare;
    private readonly Member _member;
    private readonly ExpenseProperties _expense;

    private decimal _owedPercentage;

    public ExpenseShareRowModel(ExpenseShareProperties expenseShare, Member member, ExpenseProperties expense)
    {
        _expenseShare = expenseShare;
        _member = member;
        _expense = expense;

        _owedPercentage = expenseShare.OwedAmount == 0
            ? member.DefaultSplitPercentage * 100
            : expenseShare.OwedAmount / _expense.Amount;
    }

    public Member Member => _member;

    public decimal OwedAmount
    {
        get => _expenseShare.OwedAmount;
        set
        {
            _expenseShare.OwedAmount = value;

            //_owedPercentage = _expense.Amount != 0 ? _expense.Amount * _expenseShare.OwedAmount : 0m;
        }
    }

    public decimal OwedPercentage
    {
        get => _owedPercentage;
        set => _owedPercentage = value;
    }
}
