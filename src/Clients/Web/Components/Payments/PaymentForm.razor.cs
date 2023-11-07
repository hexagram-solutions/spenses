using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Payments;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Payments;

public partial class PaymentForm
{
    [Parameter]
    public PaymentProperties Payment { get; set; } = new();

    public Validations Validations { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;
}
