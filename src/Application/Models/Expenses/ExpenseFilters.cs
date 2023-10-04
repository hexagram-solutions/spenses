namespace Spenses.Application.Models.Expenses;

public record ExpenseFilters
{
    public string[] Tags { get; set; } = Array.Empty<string>();
}
