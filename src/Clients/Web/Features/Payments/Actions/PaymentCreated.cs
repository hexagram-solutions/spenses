using BlazorState;
using Spenses.Application.Models.Payments;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Payments;

public partial class PaymentsState
{
    public record PaymentCreated(Guid HomeId, PaymentProperties Props) : IAction;

    public class PaymentCreatedHandler : ActionHandler<PaymentCreated>
    {
        private readonly IPaymentsApi _payments;

        public PaymentCreatedHandler(IStore aStore, IPaymentsApi Payments)
            : base(aStore)
        {
            _payments = Payments;
        }

        private PaymentsState PaymentsState => Store.GetState<PaymentsState>();

        public override async Task Handle(PaymentCreated aAction, CancellationToken aCancellationToken)
        {
            PaymentsState.PaymentCreating = true;

            var (homeId, props) = aAction;

            var response = await _payments.PostPayment(homeId, props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            PaymentsState.PaymentCreating = false;
        }
    }
}
