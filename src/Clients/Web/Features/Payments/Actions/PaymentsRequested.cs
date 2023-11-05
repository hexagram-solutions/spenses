using BlazorState;
using Spenses.Application.Features.Payments.Requests;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Payments;

public partial class PaymentsState
{
    public record PaymentsRequested(Guid HomeId, PaymentsQuery Query) : IAction;

    public class PaymentsRequestedHandler : ActionHandler<PaymentsRequested>
    {
        private readonly IPaymentsApi _payments;

        public PaymentsRequestedHandler(IStore aStore, IPaymentsApi payments)
            : base(aStore)
        {
            _payments = payments;
        }

        private PaymentsState PaymentState => Store.GetState<PaymentsState>();

        public override async Task Handle(PaymentsRequested aAction, CancellationToken aCancellationToken)
        {
            PaymentState.PaymentsRequesting = true;

            var paymentsResponse = await _payments.GetPayments(aAction.HomeId, aAction.Query);

            if (!paymentsResponse.IsSuccessStatusCode)
                throw new NotImplementedException();

            PaymentState.Payments = paymentsResponse.Content;

            PaymentState.PaymentsRequesting = false;
        }
    }
}
