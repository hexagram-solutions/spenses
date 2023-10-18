using System.ComponentModel;
using Spenses.Application.Models.Common;

namespace Spenses.Application.Models.Payments;

public record FilteredPaymentQuery : PagedQuery<PaymentDigest>
{
    [Description("The minimum date of payments to retrieve")]
    public DateOnly? MinDate { get; set; }

    [Description("The maximum date of payments to retrieve")]
    public DateOnly? MaxDate { get; set; }
}
