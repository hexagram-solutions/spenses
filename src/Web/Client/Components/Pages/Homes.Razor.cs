using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Web.Client.Store.Homes;

namespace Spenses.Web.Client.Components.Pages;

public partial class Homes
{
    //[Inject]
    //private IHomesApi HomesApi { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    //private Application.Models.Homes.Home[] HomesItems { get; set; } = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new HomesRequestedAction());

        //var response = await HomesApi.GetHomes();

        //HomesItems = response.Content!.ToArray();
    }
}
