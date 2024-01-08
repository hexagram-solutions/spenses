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

    private IQueryable<Shared.Models.Homes.Home> HomeItems => HomesState.Value.Homes.AsQueryable();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new HomesRequestedAction());
    }
}
