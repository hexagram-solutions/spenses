namespace Spenses.Application.Models;

public record ExpenseFilters
{
    public string[] Tags { get; set; } = Array.Empty<string>();
}
