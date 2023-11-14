using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Payments;
using Spenses.Client.Web.Store.Homes;

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
