using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.App.Store.Homes;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Payments;

namespace Spenses.App.Components.Payments;

public partial class PaymentForm
{
    [Parameter]
    [EditorRequired]
    public PaymentProperties Payment { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private DateTime? DateValue
    {
        get => Payment.Date.ToDateTime(TimeOnly.MinValue);
        set
        {
            if (value.HasValue)
                Payment.Date = DateOnly.FromDateTime(value.GetValueOrDefault());
        }
    }
}
