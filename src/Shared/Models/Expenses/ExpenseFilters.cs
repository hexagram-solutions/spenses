using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Shared.Models.Expenses;

public record ExpenseFilters
{
    [Required]
    public IEnumerable<string> Tags { get; set; } = [];

    [Required]
    public IEnumerable<ExpenseCategory> Categories { get; set; } = [];
}
