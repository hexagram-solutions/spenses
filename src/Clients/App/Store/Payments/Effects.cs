using Fluxor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.Payments;

public class Effects(IPaymentsApi payments)
{
    [EffectMethod]
    public async Task HandlePaymentRequested(PaymentRequestedAction action, IDispatcher dispatcher)
    {
        var response = await payments.GetPayment(action.HomeId, action.PaymentId);

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
        var response = await payments.PostPayment(action.HomeId, action.Props);

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
        var response = await payments.PutPayment(action.HomeId, action.PaymentId, action.Props);

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
        var response = await payments.DeletePayment(action.HomeId, action.PaymentId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new PaymentDeletionFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new PaymentDeletionSucceededAction());
    }
}
