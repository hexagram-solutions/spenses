using Fluxor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.Me;

public class Effects(IMeApi me)
{
    [EffectMethod]
    public async Task HandleCurrentUserRequested(CurrentUserRequestedAction _, IDispatcher dispatcher)
    {
        var response = await me.GetMe();

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new CurrentUserRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new CurrentUserReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleCurrentUserUpdated(CurrentUserUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await me.UpdateMe(action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new CurrentUserUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new CurrentUserUpdateSucceededAction(response.Content!));
    }
}
