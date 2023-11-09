using Fluxor;
using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Payments;

namespace Spenses.Client.Web.Store.Payments;

[FeatureState(Name = "Payments", CreateInitialStateMethodName = nameof(Initialize))]
public record PaymentsState
{
    private static PaymentsState Initialize()
    {
        return new PaymentsState();
    }

    public Payment? CurrentPayment { get; init; }

    public PagedResult<PaymentDigest> Payments { get; init; } = new(0, Enumerable.Empty<PaymentDigest>());

    public bool PaymentsRequesting { get; init; }

    public bool PaymentRequesting { get; init; }

    public bool PaymentCreating { get; init; }

    public bool PaymentUpdating { get; init; }

    public bool PaymentDeleting { get; init; }

    public bool PaymentFiltersRequesting { get; init; }

    public ApiException? Error { get; init; }
}
