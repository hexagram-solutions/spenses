using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Common.Query;

namespace Spenses.Shared.Models.Expenses;

public class ExpenseDigest
{
    [Required]
    public Guid Id { get; set; }

    public string? Note { get; set; }

    [Required]
    [Orderable]
    public DateOnly Date { get; set; }

    [Required]
    [Orderable]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    [Required]
    public string PaidByMemberId { get; set; } = null!;

    [Required]
    [Orderable]
    public string PaidByMemberName { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    [Orderable]
    public string? CategoryName { get; set; }

    [Required]
    public string CreatedByUserName { get; set; } = null!;

    [Required]
    public string ModifiedByUserName { get; set; } = null!;

    public string? Tags { get; set; }
}
