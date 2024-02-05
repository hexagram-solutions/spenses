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
    public static IdentityState ReduceLoginSucceeded(IdentityState state, LoginSucceededAction _)
    {
        return state with { LoginRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLoginFailed(IdentityState state, LoginFailedAction action)
    {
        return state with { LoginRequesting = false, Errors = [action.Error] };
    }

    [ReducerMethod]
    public static IdentityState ReduceRegistrationRequested(IdentityState state, RegistrationRequestedAction _)
    {
        return state with { RegistrationRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceRegistrationSucceeded(IdentityState state, RegistrationSucceededAction _)
    {
        return state with { RegistrationRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceRegistrationFailed(IdentityState state, RegistrationFailedAction action)
    {
        return state with { RegistrationRequesting = false, Errors = action.Errors };
    }

    [ReducerMethod]
    public static IdentityState ReduceEmailVerificationRequested(IdentityState state,
        EmailVerificationRequestedAction _)
    {
        return state with { EmailVerificationRequesting = true };
    }

    [ReducerMethod]
    public static IdentityState ReduceEmailVerificationSucceeded(IdentityState state,
        EmailVerificationSucceededAction _)
    {
        return state with { EmailVerificationRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceEmailVerificationFailed(IdentityState state,
        EmailVerificationFailedAction action)
    {
        return state with { EmailVerificationRequesting = false };
    }

    [ReducerMethod]
    public static IdentityState ReduceResendVerificationEmailRequested(IdentityState state,
        ResendVerificationEmailRequestedAction _)
    {
        return state with { ResendVerificationEmailRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceResendVerificationEmailSucceeded(IdentityState state,
        ResendVerificationEmailSucceededAction _)
    {
        return state with { ResendVerificationEmailRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceResendVerificationEmailFailed(IdentityState state,
        ResendVerificationEmailFailedAction action)
    {
        return state with { ResendVerificationEmailRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLogoutRequested(IdentityState state, LogoutRequestedAction _)
    {
        return state with { LogoutRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLogoutSucceeded(IdentityState state, LogoutSucceededAction _)
    {
        return state with { LogoutRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceLogoutFailed(IdentityState state, LogoutFailedAction action)
    {
        return state with { LogoutRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceForgotPasswordRequested(IdentityState state, ForgotPasswordRequestedAction _)
    {
        return state with { ForgotPasswordRequesting = true };
    }

    [ReducerMethod]
    public static IdentityState ReduceForgotPasswordSucceeded(IdentityState state,
        ForgotPasswordSucceededAction _)
    {
        return state with { ForgotPasswordRequesting = false };
    }

    [ReducerMethod]
    public static IdentityState ReduceForgotPasswordFailed(IdentityState state, ForgotPasswordFailedAction _)
    {
        return state with { ForgotPasswordRequesting = false };
    }

    [ReducerMethod]
    public static IdentityState ReduceResetPasswordRequested(IdentityState state, ResetPasswordRequestedAction _)
    {
        return state with { ResetPasswordRequesting = true, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceResetPasswordSucceeded(IdentityState state, ResetPasswordSucceededAction _)
    {
        return state with { ResetPasswordRequesting = false, Errors = [] };
    }

    [ReducerMethod]
    public static IdentityState ReduceResetPasswordFailed(IdentityState state, ResetPasswordFailedAction action)
    {
        return state with { ResetPasswordRequesting = false, Errors = action.Errors };
    }
}
