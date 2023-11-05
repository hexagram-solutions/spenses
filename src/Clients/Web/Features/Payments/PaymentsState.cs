using BlazorState;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Payments;

namespace Spenses.Client.Web.Features.Payments;

public partial class PaymentsState : State<PaymentsState>
{
    public Payment? CurrentPayment { get; private set; }

    public bool PaymentCreating { get; private set; }

    public bool PaymentUpdating { get; private set; }

    public bool PaymentRequesting { get; private set; }

    public bool PaymentDeleting { get; private set; }

    public PagedResult<PaymentDigest>? Payments { get; private set; }

    public bool PaymentsRequesting { get; private set; }

    public override void Initialize()
    {
        Payments = new PagedResult<PaymentDigest>(0, Array.Empty<PaymentDigest>());
    }
}
