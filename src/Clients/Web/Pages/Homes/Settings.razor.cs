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

    private HomeProperties Props { get; set; } = new();

    protected override Task OnInitializedAsync()
    {
        var currentHome = HomeState.CurrentHome;

        // todo: don't like this, have to create new models because the blazorise validator uses .GetType() under the hood
        Props = new HomeProperties
        {
            Name = currentHome.Name,
            Description = currentHome.Description,
            ExpensePeriod = currentHome.ExpensePeriod
        };

        return base.OnInitializedAsync();
    }

    private async Task Save()
    {
        if (!await Validations.ValidateAll())
            return;

        await Mediator.Send(new HomeState.HomeUpdated(HomeId, Props));
    }
}
