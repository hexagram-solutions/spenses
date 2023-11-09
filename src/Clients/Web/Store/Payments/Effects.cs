using Fluxor;
using Spenses.Client.Http;

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
            dispatcher.Dispatch(new PaymentsRequestFailedAction(response.Error));

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
            dispatcher.Dispatch(new PaymentRequestFailedAction(response.Error));

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
            dispatcher.Dispatch(new PaymentCreationFailedAction(response.Error));

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
            dispatcher.Dispatch(new PaymentUpdateFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new PaymentUpdateSucceededAction(response.Content!));
    }
}
