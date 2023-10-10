using System.ComponentModel.DataAnnotations;
using Spenses.Application.Common.Query;

namespace Spenses.Application.Models.Credits;

public class CreditDigest
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
    [Orderable]
    public string PaidByMemberName { get; set; } = null!;

    [Required]
    public string CreatedByUserName { get; set; } = null!;

    [Required]
    public string ModifiedByUserName { get; set; } = null!;
}
