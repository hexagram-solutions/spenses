using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Payments;
using Spenses.Client.Web.Store.Homes;
using Spenses.Client.Web.Store.Payments;

namespace Spenses.Client.Web.Components.Payments;

public partial class EditPaymentModal
{
    [Parameter]
    public Guid PaymentId { get; init; }

    [Inject]
    private IState<PaymentsState> PaymentsState { get; init; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; init; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private PaymentProperties Payment
    {
        get
        {
            var currentPayment = PaymentsState.Value.CurrentPayment;

            if (currentPayment is null)
            {
                return new PaymentProperties
                {
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    PaidByMemberId = Home.Members.OrderBy(m => m.Name).First().Id,
                    PaidToMemberId = Home.Members.OrderBy(m => m.Name).First().Id
                };
            }

            return new PaymentProperties
            {
                Note = currentPayment.Note,
                Date = currentPayment.Date,
                Amount = currentPayment.Amount,
                PaidByMemberId = currentPayment.PaidByMember.Id,
                PaidToMemberId = currentPayment.PaidToMember.Id
            };
        }
    }

    private PaymentForm PaymentFormRef { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new PaymentRequestedAction(Home.Id, PaymentId));
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await PaymentFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new PaymentUpdatedAction(Home.Id, PaymentId, PaymentFormRef.Payment));
    }
}
