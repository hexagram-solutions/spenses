using System.ComponentModel;
using Spenses.Application.Models.Common;

namespace Spenses.Application.Models.Expenses;

public record FilteredExpensesQuery : PagedQuery<ExpenseDigest>
{
    [Description("The minimum date of the expenses to retrieve")]
    public DateOnly? MinDate { get; set; }

    [Description("The maximum date of the expenses to retrieve")]
    public DateOnly? MaxDate { get; set; }

    [Description("Tags to filter expenses by")]
    public string[]? Tags { get; set; }
}
