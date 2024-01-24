using Fluxor;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Payments;

namespace Spenses.App.Store.Payments;

[FeatureState(Name = "Payments", CreateInitialStateMethodName = nameof(Initialize))]
public record PaymentsState
{
    private static PaymentsState Initialize()
    {
        return new PaymentsState();
    }

    public Payment? CurrentPayment { get; init; }

    public PagedResult<PaymentDigest> Payments { get; init; } = new(0, []);

    public bool PaymentsRequesting { get; init; }

    public bool PaymentRequesting { get; init; }

    public bool PaymentCreating { get; init; }

    public bool PaymentUpdating { get; init; }

    public bool PaymentDeleting { get; init; }
}
