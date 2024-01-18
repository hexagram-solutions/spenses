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
    public static MeState ReduceCurrentUserReceivedAction(MeState state, CurrentUserReceivedAction action)
    {
        return state with { CurrentUserRequesting = false, CurrentUser = action.CurrentUser };
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

    [ReducerMethod]
    public static MeState ReduceChangeEmailRequested(MeState state, ChangeEmailRequestedAction _)
    {
        return state with { ChangeEmailRequesting = true };
    }

    [ReducerMethod]
    public static MeState ReduceChangeEmailSucceededAction(MeState state, ChangeEmailSucceededAction action)
    {
        return state with { ChangeEmailRequesting = false, ChangeEmailErrors = [] };
    }

    [ReducerMethod]
    public static MeState ReduceChangeEmailRequestFailedAction(MeState state, ChangeEmailFailedAction action)
    {
        return state with { ChangeEmailRequesting = false, ChangeEmailErrors = action.Errors };
    }

    [ReducerMethod]
    public static MeState ReduceChangePasswordRequested(MeState state, ChangePasswordRequestedAction _)
    {
        return state with { ChangePasswordRequesting = true };
    }

    [ReducerMethod]
    public static MeState ReduceChangePasswordSucceededAction(MeState state, ChangePasswordSucceededAction action)
    {
        return state with { ChangePasswordRequesting = false, ChangePasswordErrors = [] };
    }

    [ReducerMethod]
    public static MeState ReduceChangePasswordRequestFailedAction(MeState state, ChangePasswordFailedAction action)
    {
        return state with { ChangePasswordRequesting = false, ChangePasswordErrors = action.Errors };
    }
}
