using System.ComponentModel.DataAnnotations;
using Spenses.Application.Models.Members;

namespace Spenses.Application.Models.Homes;

public record BalanceBreakdown
{
    [Required]
    public decimal TotalExpenses { get; set; }

    [Required]
    public decimal TotalPayments { get; set; }

    [Required]
    public decimal TotalBalance { get; set; }

    [Required]
    public IEnumerable<MemberBalance> MemberBalances { get; set; } = Array.Empty<MemberBalance>();
}

public record MemberBalance
{
    [Required]
    public Member OwedByMember { get; set; } = null!;

    [Required]
    public decimal TotalOwed { get; set; }

    [Required]
    public decimal TotalPaid { get; set; }

    [Required]
    public decimal Balance { get; set; }
}
