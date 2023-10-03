namespace Spenses.Application.Models;

public record BalanceSummary
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
