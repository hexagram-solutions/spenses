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
        var response = await _homes.GetHomes();

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomesRequestFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new HomesReceivedAction(response.Content!.ToArray()));
    }

    [EffectMethod]
    public async Task HandleHomeRequested(HomeRequestedAction action, IDispatcher dispatcher)
    {
        var response = await _homes.GetHome(action.HomeId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomeRequestFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new HomeReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleHomeCreated(HomeCreatedAction action, IDispatcher dispatcher)
    {
        var response = await _homes.PostHome(action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomeCreationFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new HomeCreationSucceededAction(response.Content!));
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
        var response = await _homes.PutHome(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomeUpdateFailedAction(response.Error));

            return;
        }

        dispatcher.Dispatch(new HomeUpdateSucceededAction(response.Content!));
    }
}
