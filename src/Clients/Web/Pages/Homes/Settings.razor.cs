using BlazorState;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Components.Homes;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Pages.Homes;

public partial class Settings : BlazorStateComponent
{
    [Parameter]
    public Guid HomeId { get; set; }

    private HomeState HomeState => GetState<HomeState>();

    public HomeForm HomeFormRef { get; set; } = null!;

    private Home Home => HomeState.CurrentHome!;

    protected override async Task OnInitializedAsync()
    {
        await Mediator.Send(new HomeState.HomeSelected(HomeId));

        await base.OnInitializedAsync();
    }

    private async Task Save()
    {
        if (!await HomeFormRef.Validations.ValidateAll())
            return;

        await Mediator.Send(new HomeState.HomeUpdated(HomeId, Home));
    }
}
