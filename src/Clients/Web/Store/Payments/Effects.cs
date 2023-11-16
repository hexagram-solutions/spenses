using Fluxor;
using Spenses.Client.Http;
using Spenses.Client.Web.Infrastructure;
using Spenses.Client.Web.Store.Shared;

namespace Spenses.Client.Web.Store.Payments;

public class Effects
{
    private readonly IPaymentsApi _payments;

    public Effects(IPaymentsApi payments)
    {
        _payments = payments;
    }

    [EffectMethod]
    public async Task HandlePaymentsRequested(PaymentsRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _payments.GetPayments(action.HomeId, action.Query);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new PaymentsRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new PaymentsReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandlePaymentRequested(PaymentRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _payments.GetPayment(action.HomeId, action.PaymentId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new PaymentRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new PaymentReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandlePaymentCreated(PaymentCreatedAction action, IDispatcher dispatcher)
    {
        var response = await _payments.PostPayment(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new PaymentCreationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new PaymentCreationSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandlePaymentUpdated(PaymentUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await _payments.PutPayment(action.HomeId, action.PaymentId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new PaymentUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new PaymentUpdateSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandlePaymentDeleted(PaymentDeletedAction action, IDispatcher dispatcher)
    {
        var response = await _payments.DeletePayment(action.HomeId, action.PaymentId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new PaymentDeletionFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new PaymentDeletionSucceededAction());
    }
}
