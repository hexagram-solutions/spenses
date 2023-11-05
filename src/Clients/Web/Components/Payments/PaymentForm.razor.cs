using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Payments;
using Spenses.Client.Web.Features.Homes;
using Spenses.Client.Web.Features.Payments;

namespace Spenses.Client.Web.Components.Payments;

public partial class PaymentForm
{
    [Parameter]
    public PaymentProperties Payment { get; set; } = new();

    private Home Home => GetState<HomeState>().CurrentHome!;

    private PaymentsState PaymentsState => GetState<PaymentsState>();
}
