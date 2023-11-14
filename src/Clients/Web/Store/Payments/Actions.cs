using Refit;
using Spenses.Application.Models.Common;
using Spenses.Application.Models.Payments;

namespace Spenses.Client.Web.Store.Payments;

public record PaymentsRequestedAction(Guid HomeId, FilteredPaymentQuery Query);

public record PaymentsReceivedAction(PagedResult<PaymentDigest> Payments);

public record PaymentsRequestFailedAction(ApiException Error);

public record PaymentRequestedAction(Guid HomeId, Guid PaymentId);

public record PaymentReceivedAction(Payment Payment);

public record PaymentRequestFailedAction(ApiException Error);

public record PaymentCreatedAction(Guid HomeId, PaymentProperties Props);

public record PaymentCreationSucceededAction(Payment Payment);

public record PaymentCreationFailedAction(ApiException Error);

public record PaymentUpdatedAction(Guid HomeId, Guid PaymentId, PaymentProperties Props);

public record PaymentUpdateSucceededAction(Payment Payment);

public record PaymentUpdateFailedAction(ApiException Error);

public record PaymentDeletedAction(Guid HomeId, Guid PaymentId);

public record PaymentDeletionSucceededAction;

public record PaymentDeletionFailedAction(ApiException Error);
