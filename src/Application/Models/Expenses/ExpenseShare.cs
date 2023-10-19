using System.ComponentModel.DataAnnotations;
using Spenses.Application.Models.Members;

namespace Spenses.Application.Models.Expenses;

public record ExpenseShare : ExpenseShareProperties
{
    public Guid Id { get; set; }

    [Required]
    public Member OwedByMember { get; set; } = null!;
}

public record ExpenseShareBase
{
    [Required]
    public Guid OwedByMemberId { get; set; }
}

public record ExpenseShareProperties : ExpenseShareBase
{

    [Required]
    [Range(0, 999_999.99)]
    public decimal OwedAmount { get; set; }

    [Required]
    [Range(0.00, 1.00)]
    public decimal OwedPercentage { get; set; }

}
