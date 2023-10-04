using System.ComponentModel.DataAnnotations;
using Spenses.Application.Common.Query;

namespace Spenses.Application.Models;

public class ExpenseDigest
{
    [Required]
    public Guid Id { get; set; }

    public string? Description { get; set; }

    [Required]
    [Orderable]
    public DateOnly Date { get; set; }

    [Required]
    [Orderable]
    [Range(0, 999_999.99)]
    public decimal Amount { get; set; }

    [Required]
    public string IncurredByMemberName { get; set; } = null!;

    [Required]
    public string CreatedByName { get; set; } = null!;

    [Required]
    public string ModifiedByName { get; set; } = null!;
    
    public string? Tags { get; set; }
}
