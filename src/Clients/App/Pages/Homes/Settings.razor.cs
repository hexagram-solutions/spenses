using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.App.Components.Homes;
using Spenses.App.Store.Homes;
using Spenses.Shared.Models.Homes;

namespace Spenses.App.Pages.Homes;

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
}
