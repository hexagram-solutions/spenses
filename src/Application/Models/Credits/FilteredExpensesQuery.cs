using System.ComponentModel;
using Spenses.Application.Models.Common;

namespace Spenses.Application.Models.Credits;

public record FilteredCreditsQuery : PagedQuery<CreditDigest>
{
    [Description("The minimum date of credits to retrieve")]
    public DateOnly? MinDate { get; set; }

    [Description("The maximum date of credits to retrieve")]
    public DateOnly? MaxDate { get; set; }
}
