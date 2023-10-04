using Spenses.Application.Models.Members;

namespace Spenses.Application.Models.Homes;

public record BalanceBreakdown
{
    public decimal TotalExpenses { get; set; }

    public decimal TotalCredits { get; set; }

    public decimal TotalBalance { get; set; }

    public MemberBalance[] Balances { get; set; } = Array.Empty<MemberBalance>();
}

public record MemberBalance
{
    public Member OwedByMember { get; set; } = null!;

    public decimal TotalOwed { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal Balance { get; set; }
}
