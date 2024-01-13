using Fluxor;

namespace Spenses.App.Store.Me;

public static class Reducers
{
    [ReducerMethod]
    public static MeState ReduceCurrentUserRequested(MeState state, CurrentUserRequestedAction _)
    {
        return state with { CurrentUserRequesting = true };
    }

    [ReducerMethod]
    public static MeState ReduceCurrentUserSucceededAction(MeState state, CurrentUserReceivedAction action)
    {
        return state with { CurrentUserRequesting = false, CurrentUser = action.CurrentUser};
    }

    [ReducerMethod]
    public static MeState ReduceCurrentUserRequestFailedAction(MeState state, CurrentUserRequestFailedAction _)
    {
        return state with { CurrentUserRequesting = false };
    }

    [ReducerMethod]
    public static MeState ReduceCurrentUserUpdated(MeState state, CurrentUserUpdatedAction _)
    {
        return state with { CurrentUserUpdating = true };
    }

    [ReducerMethod]
    public static MeState ReduceCurrentUserUpdateSucceededAction(MeState state, CurrentUserUpdateSucceededAction action)
    {
        return state with { CurrentUserUpdating = false, CurrentUser = action.CurrentUser };
    }

    [ReducerMethod]
    public static MeState ReduceCurrentUserUpdateFailedAction(MeState state, CurrentUserUpdateFailedAction _)
    {
        return state with { CurrentUserUpdating = false };
    }
}
