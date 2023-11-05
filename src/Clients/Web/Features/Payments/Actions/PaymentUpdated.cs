using BlazorState;
using Spenses.Application.Models.Payments;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Payments;

public partial class PaymentsState
{
    public record PaymentUpdated(Guid HomeId, Guid PaymentId, PaymentProperties Props) : IAction;

    public class PaymentUpdatedHandler : ActionHandler<PaymentUpdated>
    {
        private readonly IPaymentsApi _payments;

        public PaymentUpdatedHandler(IStore aStore, IPaymentsApi payments)
            : base(aStore)
        {
            _payments = payments;
        }

        private PaymentsState PaymentsState => Store.GetState<PaymentsState>();

        public override async Task Handle(PaymentUpdated aAction, CancellationToken aCancellationToken)
        {
            PaymentsState.PaymentUpdating = true;

            var (homeId, paymentId, props) = aAction;

            var response = await _payments.PutPayment(homeId, paymentId, props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            PaymentsState.PaymentUpdating = false;
        }
    }
}
