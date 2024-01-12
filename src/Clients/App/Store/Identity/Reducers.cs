using Fluxor;

namespace Spenses.App.Store.Identity;

public static class Reducers
{
    [ReducerMethod]
    public static IdentityState ReduceLoginRequested(IdentityState state, LoginRequestedAction _)
    {
        return state with { LoginRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLoginSucceededAction(IdentityState state, LoginSucceededAction _)
    {
        return state with { LoginRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLoginFailedAction(IdentityState state, LoginFailedAction action)
    {
        return state with { LoginRequesting = false, Errors = [action.Error] };
    }

    [ReducerMethod]
    public static IdentityState ReduceRegistrationRequested(IdentityState state, RegistrationRequestedAction _)
    {
        return state with { RegistrationRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceRegistrationSucceededAction(IdentityState state, RegistrationSucceededAction _)
    {
        return state with { RegistrationRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceRegistrationFailedAction(IdentityState state, RegistrationFailedAction action)
    {
        return state with { RegistrationRequesting = false, Errors = action.Errors };
    }

    [ReducerMethod]
    public static IdentityState ReduceEmailVerificationRequested(IdentityState state,
        EmailVerificationRequestedAction _)
    {
        return state with { EmailVerificationRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceEmailVerificationSucceededAction(IdentityState state,
        EmailVerificationSucceededAction _)
    {
        return state with { EmailVerificationRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceEmailVerificationFailedAction(IdentityState state,
        EmailVerificationFailedAction action)
    {
        return state with { EmailVerificationRequesting = false, Errors = [action.Error] };
    }

    [ReducerMethod]
    public static IdentityState ReduceResendVerificationEmailRequested(IdentityState state,
        ResendVerificationEmailRequestedAction _)
    {
        return state with { ResendVerificationEmailRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceResendVerificationEmailSucceededAction(IdentityState state,
        ResendVerificationEmailSucceededAction _)
    {
        return state with { ResendVerificationEmailRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceResendVerificationEmailFailedAction(IdentityState state,
        ResendVerificationEmailFailedAction action)
    {
        return state with { ResendVerificationEmailRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLogoutRequested(IdentityState state,
        LogoutRequestedAction _)
    {
        return state with { LogoutRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLogoutSucceededAction(IdentityState state,
        LogoutSucceededAction _)
    {
        return state with { LogoutRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLogoutFailedAction(IdentityState state,
        LogoutFailedAction action)
    {
        return state with { LogoutRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceForgotPasswordRequested(IdentityState state,
        ForgotPasswordRequestedAction _)
    {
        return state with { ForgotPasswordRequesting = true };
    }

    [ReducerMethod]
    public static IdentityState ReduceForgotPasswordSucceededAction(IdentityState state,
        ForgotPasswordSucceededAction _)
    {
        return state with { ForgotPasswordRequesting = false };
    }

    [ReducerMethod]
    public static IdentityState ReduceForgotPasswordFailedAction(IdentityState state,
        ForgotPasswordFailedAction action)
    {
        return state with { ForgotPasswordRequesting = false };
    }
}
