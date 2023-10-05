using System.ComponentModel.DataAnnotations;
using Spenses.Application.Models.Members;
using Spenses.Application.Models.Users;

namespace Spenses.Application.Models.Credits;

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

public record Credit : CreditBase
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
