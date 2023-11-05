using BlazorState;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Payments;

public partial class PaymentsState
{
    public record PaymentDeleted(Guid HomeId, Guid PaymentId) : IAction;

    public class PaymentDeletedHandler : ActionHandler<PaymentDeleted>
    {
        private readonly IPaymentsApi _payments;

        public PaymentDeletedHandler(IStore aStore, IPaymentsApi payments)
            : base(aStore)
        {
            _payments = payments;
        }

        private PaymentsState PaymentsState => Store.GetState<PaymentsState>();

        public override async Task Handle(PaymentDeleted aAction, CancellationToken aCancellationToken)
        {
            PaymentsState.PaymentDeleting = true;

            var (homeId, paymentId) = aAction;

            var response = await _payments.DeletePayment(homeId, paymentId);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            PaymentsState.PaymentDeleting = false;
        }
    }
}
