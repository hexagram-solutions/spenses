using Fluxor;

namespace Spenses.App.Store.Payments;

public static class Reducers
{

    [ReducerMethod]
    public static PaymentsState ReducePaymentRequested(PaymentsState state, PaymentRequestedAction _)
    {
        return state with { PaymentRequesting = true };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentReceived(PaymentsState state, PaymentReceivedAction action)
    {
        return state with { PaymentRequesting = false, CurrentPayment = action.Payment };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentRequestedFailed(PaymentsState state, PaymentRequestFailedAction action)
    {
        return state with { PaymentRequesting = false };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentCreated(PaymentsState state, PaymentCreatedAction _)
    {
        return state with { PaymentCreating = true };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentCreationSucceeded(PaymentsState state,
        PaymentCreationSucceededAction action)
    {
        return state with { PaymentCreating = false, CurrentPayment = action.Payment };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentCreationFailed(PaymentsState state, PaymentCreationFailedAction action)
    {
        return state with { PaymentCreating = false };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentUpdated(PaymentsState state, PaymentUpdatedAction _)
    {
        return state with { PaymentUpdating = true };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentUpdateSucceeded(PaymentsState state, PaymentUpdateSucceededAction action)
    {
        return state with { PaymentUpdating = false, CurrentPayment = action.Payment };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentUpdateFailed(PaymentsState state, PaymentUpdateFailedAction action)
    {
        return state with { PaymentUpdating = false };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentDeleted(PaymentsState state, PaymentDeletedAction _)
    {
        return state with { PaymentDeleting = true };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentDeleteSucceeded(PaymentsState state, PaymentDeletionSucceededAction _)
    {
        return state with { PaymentDeleting = false };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentDeleteFailed(PaymentsState state, PaymentDeletionFailedAction action)
    {
        return state with { PaymentDeleting = false };
    }
}
