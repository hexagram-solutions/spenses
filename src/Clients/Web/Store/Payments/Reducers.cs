using Fluxor;

namespace Spenses.Client.Web.Store.Payments;

public static class Reducers
{
    [ReducerMethod]
    public static PaymentsState ReducePaymentsRequested(PaymentsState state, PaymentsRequestedAction _)
    {
        return state with { PaymentsRequesting = true };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentsReceived(PaymentsState state, PaymentsReceivedAction action)
    {
        return state with { PaymentsRequesting = false, Payments = action.Payments };
    }

    [ReducerMethod]
    public static PaymentsState ReducePaymentsRequestFailed(PaymentsState state, PaymentsRequestFailedAction action)
    {
        return state with { PaymentsRequesting = false, Error = action.Error };
    }

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
        return state with { PaymentRequesting = false, Error = action.Error };
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
        return state with { PaymentCreating = false, Error = action.Error };
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
        return state with { PaymentUpdating = false, Error = action.Error };
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
        return state with { PaymentDeleting = false, Error = action.Error };
    }
}
