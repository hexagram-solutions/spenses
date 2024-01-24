using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Members;

namespace Spenses.Shared.Models.Homes;

public record BalanceBreakdown
{
    /// <summary>
    /// The sum of all expenses in the balance period.
    /// </summary>
    [Required]
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// The balances for each member of the home.
    /// </summary>
    [Required]
    public MemberBalance[] MemberBalances { get; set; } = [];
}

public record MemberBalance
{
    /// <summary>
    /// The home member.
    /// </summary>
    [Required]
    public Member Member { get; set; } = null!;

    /// <summary>
    /// Debts that the member owes to other members of the home.
    /// </summary>
    [Required]
    public MemberDebt[] Debts { get; set; } = [];
}

public record MemberDebt
{
    /// <summary>
    /// The home member to whom payment is owed.
    /// </summary>
    [Required]
    public Member OwedTo { get; set; } = null!;

    /// <summary>
    /// The total amount owed to the other home member for the period.
    /// </summary>
    [Required]
    public decimal TotalOwed { get; set; }

    /// <summary>
    /// The total amount of payments the member made to the other member for the period.
    /// </summary>
    [Required]
    public decimal TotalPaid { get; set; }

    /// <summary>
    /// The remaining balance owing to the other home member for the period.
    /// </summary>
    [Required]
    public decimal BalanceOwing { get; set; }
}
