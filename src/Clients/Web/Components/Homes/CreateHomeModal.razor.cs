using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Homes;

public partial class CreateHomeModal
{
    [Inject]
    private IModalService ModalService { get; init; } = null!;

    [Parameter]
    public HomeProperties Home { get; set; } = null!;

    private Validations Validations { get; set; } = null!;

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Create()
    {
        if (!await Validations.ValidateAll())
            return;

        await Mediator.Send(new HomeState.HomeCreated(Home));

        await Close();
    }
}
