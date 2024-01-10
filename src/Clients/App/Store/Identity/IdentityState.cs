using System;
using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Spenses.App.Authentication;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Store.Identity;

[FeatureState(Name = "Identity", CreateInitialStateMethodName = nameof(Initialize))]
public record IdentityState
{
    public bool LoginRequesting { get; init; }

    public bool RegistrationRequesting { get; init; }

    public bool EmailVerificationRequesting { get; init; }

    public bool ResendVerificationEmailRequesting { get; init; }

    public string[] Errors { get; init; }

    private static IdentityState Initialize()
    {
        return new IdentityState();
    }
}

public class Effects(IIdentityApi identityApi, IAuthenticationService authenticationService)
{
    [EffectMethod]
    public async Task HandleLoginRequested(LoginRequestedAction action, IDispatcher dispatcher)
    {
        var result = await authenticationService.Login(action.Request);

        if (result.Error is not null)
        {
            dispatcher.Dispatch(new LoginFailedAction("Your email or password was incorrect. Please try again."));

            return;
        }

        if (result.Content!.Succeeded)
        {
            dispatcher.Dispatch(new LoginSucceededAction());
            dispatcher.Dispatch(new GoAction(action.ReturnUrl ?? Routes.Root));
        }
        else if (result.Content.RequiresTwoFactor)
        {
            dispatcher.Dispatch(new GoAction(Routes.Identity.TwoFactorLogin(action.ReturnUrl)));
        }
        else if (result.Content.IsLockedOut)
        {
            dispatcher.Dispatch(new LoginFailedAction("This account is locked."));
        }
    }

    [EffectMethod]
    public async Task HandleRegistrationRequested(RegistrationRequestedAction action, IDispatcher dispatcher)
    {
        var result = await authenticationService.Register(action.Request);

        if (result.Succeeded)
        {
            dispatcher.Dispatch(new RegistrationSucceededAction());
            dispatcher.Dispatch(new GoAction(Routes.Identity.EmailVerificationRequired));

            return;
        }

        if (result.Error!.Errors.ContainsKey(IdentityErrors.DuplicateUserName) ||
                 result.Error.Errors.ContainsKey(IdentityErrors.DuplicateEmail))
        {
            dispatcher.Dispatch(new RegistrationFailedAction([
                "It looks like you may already have an account with " +
                "us. Use your credentials to log in instead."
            ]));

            return;
        }

        if (result.Error.Errors.Count != 0)
        {
            dispatcher.Dispatch(new RegistrationFailedAction(result.Error.Errors.SelectMany(e => e.Value).ToArray()));

            return;
        }

        dispatcher.Dispatch(new RegistrationFailedAction(["An unknown error occurred."]));
        dispatcher.Dispatch(new ApplicationErrorAction(result.Error.ToApplicationError()));
    }
}

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
        return state with { ResendVerificationEmailRequesting = false, Errors = [action.Error] };
    }
}
