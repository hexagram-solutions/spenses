using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Payments;
using Spenses.Client.Web.Features.Homes;
using Spenses.Client.Web.Features.Payments;

namespace Spenses.Client.Web.Components.Payments;

public partial class CreatePaymentModal
{
    [Parameter]
    public Func<Task> OnSave { get; set; } = null!;

    public PaymentProperties Payment { get; set; } = new();

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private PaymentForm PaymentFormRef { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private PaymentsState PaymentsState => GetState<PaymentsState>();

    protected override Task OnInitializedAsync()
    {
        var members = Home.Members.OrderBy(m => m.Name);

        Payment = new PaymentProperties
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            PaidByMemberId = members.First().Id,
            // todo: probably need to prevent a home with only one member doing stuff
            //todo, can a member pay themselves?
            PaidToMemberId = Home.Members.Skip(1).First().Id
        };

        return base.OnInitializedAsync();
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await PaymentFormRef.Validations.ValidateAll())
            return;

        await Mediator.Send(new PaymentsState.PaymentCreated(Home.Id, Payment));

        await Close();

        await OnSave();
    }
}
