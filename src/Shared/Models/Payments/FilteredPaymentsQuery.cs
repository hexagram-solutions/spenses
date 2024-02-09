using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Spenses.Shared.Models.Common;

namespace Spenses.Shared.Models.Payments;

public record FilteredPaymentsQuery : PagedQuery<PaymentDigest>
{
    [Required]
    [Description("The minimum date of payments to retrieve")]
    public DateOnly MinDate { get; set; }

    [Required]
    [Description("The maximum date of payments to retrieve")]
    public DateOnly MaxDate { get; set; }
}
