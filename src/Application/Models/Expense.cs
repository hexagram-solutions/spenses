using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public abstract record ExpenseBase
{
    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }
}

public record Expense : ExpenseBase, IAggregateRoot
{
    public Guid Id { get; set; }

    [Required]
    public Member IncurredByMember { get; set; } = null!;

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}

public record ExpenseProperties : ExpenseBase
{
    [Required]
    public Guid IncurredByMemberId { get; set; }
}
