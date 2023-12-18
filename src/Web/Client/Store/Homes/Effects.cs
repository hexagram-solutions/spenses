using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Spenses.Client.Http;
using Spenses.Web.Client.Infrastructure;
using Spenses.Web.Client.Store.Shared;

namespace Spenses.Web.Client.Store.Homes;

public class Effects(IHomesApi homes)
{
    [EffectMethod]
    public async Task HandleHomesRequested(HomesRequestedAction _, IDispatcher dispatcher)
    {
        var response = await homes.GetHomes();

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomesRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new HomesReceivedAction(response.Content!.ToArray()));
    }

    [EffectMethod]
    public async Task HandleHomeRequested(HomeRequestedAction action, IDispatcher dispatcher)
    {
        var response = await homes.GetHome(action.HomeId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomeRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new HomeReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleHomeCreated(HomeCreatedAction action, IDispatcher dispatcher)
    {
        var response = await homes.PostHome(action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomeCreationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new HomeCreationSucceededAction(response.Content!));
    }

    [EffectMethod]
    public Task HandleHomeCreationSucceeded(HomeCreationSucceededAction action, IDispatcher dispatcher)
    {
        dispatcher.Dispatch(new HomesRequestedAction());
        dispatcher.Dispatch(new GoAction($"homes/{action.Home.Id}/dashboard"));

        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleHomeUpdated(HomeUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await homes.PutHome(action.HomeId, action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new HomeUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new HomeUpdateSucceededAction(response.Content!));
    }
}
