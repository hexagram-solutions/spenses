using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using MudBlazor;
using Spenses.App.Store.Homes;
using Spenses.App.Store.Payments;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Payments;

namespace Spenses.App.Components.Payments;

public partial class CreatePaymentDialog
{
    [CascadingParameter]
    private MudDialogInstance Dialog { get; set; } = null!;

    [Inject]
    private IState<PaymentsState> PaymentsState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    public PaymentProperties Payment { get; set; } = new()
    {
        Amount = 0.00m,
        Date = DateOnly.FromDateTime(DateTime.Today)
    };

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new PaymentCreatedAction(Home.Id, Payment));
    }
}
