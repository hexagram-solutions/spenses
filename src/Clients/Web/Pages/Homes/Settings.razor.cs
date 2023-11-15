using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Client.Web.Components.Homes;
using Spenses.Client.Web.Store.Homes;

namespace Spenses.Client.Web.Pages.Homes;

public partial class Settings
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    public HomeForm HomeFormRef { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome ?? new Home();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (HomesState.Value.CurrentHome?.Id == HomeId)
            return;

        Dispatcher.Dispatch(new HomeRequestedAction(HomeId));
    }

    private async Task Save()
    {
        if (!await HomeFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new HomeUpdatedAction(HomeId, Home!));
    }
}
