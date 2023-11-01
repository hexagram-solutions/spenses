using BlazorState;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Pages.Homes;

public partial class Settings : BlazorStateComponent
{
    [Parameter]
    public Guid HomeId { get; set; }

    private HomeState HomeState => GetState<HomeState>();

    private Validations Validations { get; set; } = new();

    private Home Home => HomeState.CurrentHome!;

    protected override Task OnParametersSetAsync()
    {
        return Mediator.Send(new HomeState.HomeSelected(HomeId));
    }

    private async Task Save()
    {
        if (!await Validations.ValidateAll())
            return;

        await Mediator.Send(new HomeState.HomeUpdated(HomeId, Home));
    }
}
