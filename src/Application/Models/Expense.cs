using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public abstract record ExpenseBase
{
    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public decimal Amount { get; set; }
}

public record Expense : ExpenseBase
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Member IncurredByMember { get; set; } = null!;
}

public record ExpenseProperties : ExpenseBase
{
    [Required]
    public Guid IncurredByMemberId { get; set; }
}
