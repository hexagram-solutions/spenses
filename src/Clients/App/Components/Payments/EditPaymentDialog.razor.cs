using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Store.Homes;
using Spenses.App.Store.Payments;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Payments;

namespace Spenses.App.Components.Payments;

public partial class EditPaymentDialog
{
    [Parameter]
    [EditorRequired]
    public Guid PaymentId { get; set; }

    [CascadingParameter]
    private MudDialogInstance Dialog { get; set; } = null!;

    [Inject]
    private IState<PaymentsState> PaymentsState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;
    public PaymentProperties Payment { get; set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<PaymentReceivedAction>(a => Payment = new PaymentProperties
        {
            Amount = a.Payment.Amount,
            Date = a.Payment.Date,
            Note = a.Payment.Note,
            PaidByMemberId = a.Payment.PaidByMember.Id,
            PaidToMemberId = a.Payment.PaidToMember.Id
        });

        Dispatcher.Dispatch(new PaymentRequestedAction(Home.Id, PaymentId));
    }

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save()
    {
        Dispatcher.Dispatch(new PaymentUpdatedAction(Home.Id, PaymentId, Payment));
    }
}
