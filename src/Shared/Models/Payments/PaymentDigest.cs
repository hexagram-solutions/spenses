using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Common.Query;

namespace Spenses.Shared.Models.Payments;

public class PaymentDigest
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [Orderable]
    public DateOnly Date { get; set; }

    [Required]
    [Orderable]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    public string? Note { get; set; }

    [Required]
    public Guid PaidByMemberId { get; set; }

    [Required]
    [Orderable]
    public string PaidByMemberName { get; set; } = null!;

    [Required]
    public Guid PaidToMemberId { get; set; }

    [Required]
    [Orderable]
    public string PaidToMemberName { get; set; } = null!;

    [Required]
    public string CreatedByUserName { get; set; } = null!;

    [Required]
    public string ModifiedByUserName { get; set; } = null!;
}
