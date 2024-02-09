using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Common;

namespace Spenses.Shared.Models.Expenses;

public record FilteredExpensesQuery : PagedQuery<ExpenseDigest>
{
    [Required]
    [Description("The minimum date of expenses to retrieve")]
    public DateOnly MinDate { get; set; }

    [Required]
    [Description("The maximum date of expenses to retrieve")]
    public DateOnly MaxDate { get; set; }

    [Description("Tags to filter expenses by")]
    public IEnumerable<string>? Tags { get; set; }

    [Description("Identifiers of categories to filter expenses by")]
    public IEnumerable<Guid>? Categories { get; set; }
}
