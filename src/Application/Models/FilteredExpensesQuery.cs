using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace Spenses.Application.Models;

public record FilteredExpensesQuery : PagedQuery<ExpenseDigest>
{
    [FromQuery(Name = "minDate")]
    [Description("The minimum date of the expenses to retrieve")]
    public DateOnly? MinDate { get; set; }

    [FromQuery(Name = "maxDate")]
    [Description("The maximum date of the expenses to retrieve")]
    public DateOnly? MaxDate { get; set; }

    [FromQuery(Name = "tags")]
    [Description("Tags to filter expenses by")]
    public string[]? Tags { get; set; }
}
