using BlazorState;
using Spenses.Application.Models.Payments;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Payments;

public partial class PaymentsState
{
    public record PaymentSelected(Guid HomeId, Guid PaymentId) : IAction;

    public class PaymentSelectedHandler : ActionHandler<PaymentSelected>
    {
        private readonly IPaymentsApi _payments;

        public PaymentSelectedHandler(IStore aStore, IPaymentsApi payments)
            : base(aStore)
        {
            _payments = payments;
        }

        private PaymentsState PaymentsState => Store.GetState<PaymentsState>();

        public override async Task Handle(PaymentSelected aAction, CancellationToken aCancellationToken)
        {
            PaymentsState.PaymentRequesting = true;

            var (homeId, paymentId) = aAction;

            var response = await _payments.GetPayment(homeId, paymentId);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            PaymentsState.CurrentPayment = response.Content;

            PaymentsState.PaymentRequesting = false;
        }
    }
}
