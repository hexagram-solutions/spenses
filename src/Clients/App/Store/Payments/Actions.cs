using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Payments;

namespace Spenses.App.Store.Payments;

public record PaymentsRequestedAction(Guid HomeId, FilteredPaymentQuery Query);

public record PaymentsReceivedAction(PagedResult<PaymentDigest> Payments);

public record PaymentsRequestFailedAction;

public record PaymentRequestedAction(Guid HomeId, Guid PaymentId);

public record PaymentReceivedAction(Payment Payment);

public record PaymentRequestFailedAction;

public record PaymentCreatedAction(Guid HomeId, PaymentProperties Props);

public record PaymentCreationSucceededAction(Payment Payment);

public record PaymentCreationFailedAction;

public record PaymentUpdatedAction(Guid HomeId, Guid PaymentId, PaymentProperties Props);

public record PaymentUpdateSucceededAction(Payment Payment);

public record PaymentUpdateFailedAction;

public record PaymentDeletedAction(Guid HomeId, Guid PaymentId);

public record PaymentDeletionSucceededAction;

public record PaymentDeletionFailedAction;
