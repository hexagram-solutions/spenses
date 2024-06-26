using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Members;

namespace Spenses.Shared.Models.Expenses;

public record ExpenseShare : ExpenseShareBase
{
    public Guid Id { get; set; }

    [Required]
    public Member OwedByMember { get; set; } = null!;
}

public record ExpenseShareBase
{
    [Required]
    [Range(0, 999_999.99)]
    public decimal OwedAmount { get; set; }

    [Required]
    [Range(0.00, 1.00)]
    public decimal OwedPercentage { get; set; }
}

public record ExpenseShareProperties : ExpenseBase
{
    [Required]
    public Guid OwedByMemberId { get; set; }
}
