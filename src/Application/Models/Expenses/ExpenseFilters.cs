using System.ComponentModel.DataAnnotations;

namespace Spenses.Application.Models.Expenses;

public record ExpenseFilters
{
    [Required]
    public string[] Tags { get; set; } = Array.Empty<string>();

    [Required]
    public Dictionary<Guid, string> Categories { get; set; } = new();
}
