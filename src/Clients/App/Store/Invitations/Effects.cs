using System.Net;
using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;

namespace Spenses.App.Store.Invitations;

public class Effects(IIdentityApi identityApi, IInvitationsApi invitations)
{
    [EffectMethod]
    public async Task HandleInvitationRequested(InvitationRequestedAction action, IDispatcher dispatcher)
    {
        var response = await identityApi.GetInvitation(action.Token);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new InvitationRequestFailedAction());

            if (response.StatusCode is not HttpStatusCode.NotFound)
                dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new InvitationReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleInvitationAccepted(InvitationAcceptedAction action, IDispatcher dispatcher)
    {
        var response = await invitations.AcceptInvitation(action.InvitationId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new InvitationAcceptanceFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new InvitationAcceptanceSucceededAction());
        dispatcher.Dispatch(new GoAction(Routes.Homes.Dashboard(response.Content!.Home.Id), true));
    }

    [EffectMethod]
    public async Task HandleInvitationDeclined(InvitationDeclinedAction action, IDispatcher dispatcher)
    {
        var response = await invitations.DeclineInvitation(action.InvitationId);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new InvitationDeclinationFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new InvitationDeclinationSucceededAction());
        dispatcher.Dispatch(new GoAction(Routes.Root));
    }
}
