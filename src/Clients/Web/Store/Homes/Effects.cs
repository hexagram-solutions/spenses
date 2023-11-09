using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Store.Homes;

public class Effects
{
    private readonly IHomesApi _homes;
    private readonly NavigationManager _navigationManager;

    public Effects(IHomesApi homes, NavigationManager navigationManager)
    {
        _homes = homes;
        _navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task HandleHomesRequested(HomesRequestedAction _, IDispatcher dispatcher)
    {
        var homesResponse = await _homes.GetHomes();

        if (!homesResponse.IsSuccessStatusCode)
            dispatcher.Dispatch(new HomesRequestFailedAction(homesResponse.Error.ReasonPhrase!));

        dispatcher.Dispatch(new HomesReceivedAction(homesResponse.Content!));
    }

    [EffectMethod]
    public async Task HandleHomeRequested(HomeRequestedAction action, IDispatcher dispatcher)
    {
        var homeResponse = await _homes.GetHome(action.HomeId);

        if (!homeResponse.IsSuccessStatusCode)
            dispatcher.Dispatch(new HomeRequestFailedAction(homeResponse.Error.ReasonPhrase!));

        dispatcher.Dispatch(new HomeReceivedAction(homeResponse.Content!));
    }

    [EffectMethod]
    public async Task HandleHomeCreated(HomeCreatedAction action, IDispatcher dispatcher)
    {
        var homeResponse = await _homes.PostHome(action.Props);

        if (!homeResponse.IsSuccessStatusCode)
            dispatcher.Dispatch(new HomeCreationFailedAction(homeResponse.Error.ReasonPhrase!));

        dispatcher.Dispatch(new HomeCreationSucceededAction(homeResponse.Content!));
    }
    
    [EffectMethod]
    public Task HandleHomeCreationSucceeded(HomeCreationSucceededAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new HomesRequestedAction());

        _navigationManager.NavigateTo($"homes/{action.Home.Id}/dashboard");

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleHomeUpdated(HomeUpdatedAction action, IDispatcher dispatcher)
    {
        var homeResponse = await _homes.PutHome(action.HomeId, action.Props);

        if (!homeResponse.IsSuccessStatusCode)
            dispatcher.Dispatch(new HomeUpdateFailedAction(homeResponse.Error.ReasonPhrase!));

        dispatcher.Dispatch(new HomeUpdateSucceededAction(homeResponse.Content!));
    }
}
