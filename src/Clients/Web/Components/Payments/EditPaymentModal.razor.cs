using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Payments;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Payments;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Payments;

public partial class EditPaymentModal
{
    [Parameter]
    public Guid PaymentId { get; set; }

    [Parameter]
    public Func<Task> OnSave { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Validations Validations { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private PaymentsState PaymentsState => GetState<PaymentsState>();

    private PaymentProperties Payment { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await Mediator.Send(new PaymentsState.PaymentSelected(Home.Id, PaymentId));

        var currentPayment = PaymentsState.CurrentPayment!;

        Payment = new PaymentProperties
        {
            Amount = currentPayment.Amount,
            Date = currentPayment.Date,
            Note = currentPayment.Note,
            PaidByMemberId = currentPayment.PaidByMember.Id,
            PaidToMemberId = currentPayment.PaidToMember.Id,
        };

        await base.OnInitializedAsync();
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await Validations.ValidateAll())
            return;

        await Mediator.Send(new PaymentsState.PaymentUpdated(Home.Id, PaymentId, Payment));

        await Close();

        await OnSave();
    }
}
