using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Spenses.App.Authentication;
using Spenses.App.Infrastructure;
using Spenses.Client.Http;

namespace Spenses.App.Store.Identity;

[FeatureState(Name = "Identity", CreateInitialStateMethodName = nameof(Initialize))]
public record IdentityState
{
    private static IdentityState Initialize()
    {
        return new IdentityState();
    }

    public bool IsLoggingIn { get; init; }

    public string? Error { get; init; }
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
}

public static class Reducers
{
    [ReducerMethod]
    public static IdentityState ReduceLoginRequested(IdentityState state, LoginRequestedAction action)
    {
        return state with { IsLoggingIn = true, Error = null };
    }

    [ReducerMethod]
    public static IdentityState ReduceLoginSucceededAction(IdentityState state, LoginSucceededAction _)
    {
        return state with { IsLoggingIn = false, Error = null };
    }

    [ReducerMethod]
    public static IdentityState ReduceLoginFailedAction(IdentityState state, LoginFailedAction action)
    {
        return state with { IsLoggingIn = false, Error = action.Error};
    }
}

