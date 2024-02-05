using Fluxor;

namespace Spenses.App.Store.Invitations;

public static class Reducers
{
    [ReducerMethod]
    public static InvitationState ReduceInvitationRequested(InvitationState state, InvitationRequestedAction _)
    {
        return state with { InvitationRequesting = true };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationReceived(InvitationState state, InvitationReceivedAction action)
    {
        return state with { InvitationRequesting = false, Invitation = action.Invitation };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationFailed(InvitationState state, InvitationRequestFailedAction _)
    {
        return state with { InvitationRequesting = false };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationAccepted(InvitationState state, InvitationAcceptedAction _)
    {
        return state with { InvitationResponding = true };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationAcceptanceSucceeded(InvitationState state,
        InvitationAcceptanceSucceededAction _)
    {
        return state with { InvitationResponding = false };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationAcceptanceFailed(InvitationState state,
        InvitationAcceptanceFailedAction _)
    {
        return state with { InvitationResponding = false };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationDeclined(InvitationState state, InvitationDeclinedAction _)
    {
        return state with { InvitationResponding = true };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationDeclinationSucceeded(InvitationState state,
        InvitationDeclinationSucceededAction _)
    {
        return state with { InvitationResponding = false };
    }

    [ReducerMethod]
    public static InvitationState ReduceInvitationDeclinationFailed(InvitationState state,
        InvitationDeclinationFailedAction _)
    {
        return state with { InvitationResponding = false };
    }
}
