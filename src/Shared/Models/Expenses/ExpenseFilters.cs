using System.ComponentModel.DataAnnotations;
using Spenses.Application.Models.ExpenseCategories;

namespace Spenses.Application.Models.Expenses;

public record ExpenseFilters
{
    [Required]
    public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

    [Required]
    public IEnumerable<ExpenseCategory> Categories { get; set; } = Enumerable.Empty<ExpenseCategory>();
}
