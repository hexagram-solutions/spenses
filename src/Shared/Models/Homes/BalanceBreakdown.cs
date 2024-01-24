using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Members;

namespace Spenses.Shared.Models.Homes;

public record BalanceBreakdown
{
    [Required]
    public decimal TotalExpenses { get; set; }

    [Required]
    public decimal TotalPayments { get; set; }

    [Required]
    public decimal TotalBalance { get; set; }

    [Required]
    public IEnumerable<MemberBalance> MemberBalances { get; set; } = [];
}

public record MemberBalance
{
    [Required]
    public Member Member { get; set; } = null!;

    [Required]
    public decimal TotalOwed { get; set; }

    [Required]
    public decimal TotalPaid { get; set; }

    [Required]
    public decimal Balance { get; set; }
}
