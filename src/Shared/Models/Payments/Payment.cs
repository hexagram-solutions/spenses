using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Models.Users;

namespace Spenses.Shared.Models.Payments;

public abstract record PaymentBase
{
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    public string? Note { get; set; }
}

public record PaymentProperties : PaymentBase
{
    [Required]
    public Guid PaidByMemberId { get; set; }

    [Required]
    public Guid PaidToMemberId { get; set; }
}

public record Payment : PaymentBase
{
    public Guid Id { get; set; }

    [Required]
    public Member PaidByMember { get; set; } = null!;

    [Required]
    public Member PaidToMember { get; set; } = null!;

    [Required]
    public User CreatedBy { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public User ModifiedBy { get; set; } = null!;

    [Required]
    public DateTime ModifiedAt { get; set; }
}
