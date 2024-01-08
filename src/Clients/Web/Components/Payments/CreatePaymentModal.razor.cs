using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Web.Store.Homes;
using Spenses.Client.Web.Store.Payments;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Payments;

namespace Spenses.Client.Web.Components.Payments;

public partial class CreatePaymentModal
{
    [Inject]
    private IState<PaymentsState> PaymentsState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    public PaymentProperties Payment { get; set; } = new();

    private PaymentForm PaymentFormRef { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var members = Home.Members.OrderBy(m => m.Name).ToList();

        Payment = new PaymentProperties
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            PaidByMemberId = members.First().Id,
            PaidToMemberId = members.Count > 1 ? Home.Members.Skip(1).First().Id : members.First().Id
        };
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await PaymentFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new PaymentCreatedAction(Home.Id, PaymentFormRef.Payment));
    }
}
