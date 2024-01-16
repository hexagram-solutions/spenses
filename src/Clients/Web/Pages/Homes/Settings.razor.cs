using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Spenses.Client.Web.Components.Homes;
using Spenses.Client.Web.Store.Homes;
using Spenses.Shared.Models.Homes;

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

    private void Save(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new HomeUpdatedAction(HomeId, Home!));
    }
}
