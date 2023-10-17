using BlazorState;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Homes;

public partial class CreateHomeModal : BlazorStateComponent
{
    [Inject]
    private IModalService? ModalService { get; set; }

    public HomeProperties Home { get; set; } = new();

    private Validations Validations { get; set; } = new();

    private Task Close()
    {
        return ModalService!.Hide();
    }

    private async Task Create()
    {
        if (!await Validations.ValidateAll())
            return;

        await Mediator.Send(new HomeState.HomeCreated(Home));

        await Close();
    }
}
