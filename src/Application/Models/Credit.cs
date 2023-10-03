using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models;

public abstract record CreditBase
{
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }
}

public record CreditProperties : CreditBase
{
    [Required]
    public Guid PaidByMemberId { get; set; }
}

public record Credit : CreditBase, IAggregateRoot
{
    public Guid Id { get; set; }

    [Required]
    public Member PaidByMember { get; set; } = null!;

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}
