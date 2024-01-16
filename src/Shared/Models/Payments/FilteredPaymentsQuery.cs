using System.ComponentModel;
using Spenses.Shared.Models.Common;

namespace Spenses.Shared.Models.Payments;

public record FilteredPaymentsQuery : PagedQuery<PaymentDigest>
{
    [Description("The minimum date of payments to retrieve")]
    public DateOnly? MinDate { get; set; }

    [Description("The maximum date of payments to retrieve")]
    public DateOnly? MaxDate { get; set; }
}
