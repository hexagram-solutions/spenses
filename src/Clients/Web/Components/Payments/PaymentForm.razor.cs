using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Web.Store.Homes;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Payments;

namespace Spenses.Client.Web.Components.Payments;

public partial class PaymentForm
{
    [Parameter]
    public PaymentProperties Payment { get; set; } = new();

    [Inject]
    private IState<HomesState> HomesState { get; init; } = null!;

    public Validations Validations { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;
}
