using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Web.Client.Store.Homes;

namespace Spenses.Web.Client.Components.Pages;

public partial class Homes
{
    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new HomesRequestedAction());
    }
}
