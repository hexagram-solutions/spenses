using Microsoft.AspNetCore.Components;
using Spenses.Shared.Models.Homes;

namespace Spenses.Client.Web.Components.Homes;

public partial class HomeForm
{
    [Parameter]
    public HomeProperties Home { get; set; } = null!;

    public Validations Validations { get; set; } = null!;
}
