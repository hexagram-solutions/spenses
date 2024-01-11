using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Spenses.App.Authentication;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Store.Identity;

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

    [EffectMethod]
    public async Task HandleResendVerificationEmailRequested(ResendVerificationEmailRequestedAction action,
        IDispatcher dispatcher)
    {
        var response = await identityApi.ResendVerificationEmail(new ResendVerificationEmailRequest(action.Email));

        if (!response.IsSuccessStatusCode)
        {
            dispatcher.Dispatch(new ResendVerificationEmailFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));
        }

        dispatcher.Dispatch(new ResendVerificationEmailSucceededAction());
    }

    [EffectMethod]
    public async Task HandleLogoutRequestedAction(LogoutRequestedAction action,
        IDispatcher dispatcher)
    {
        var result = await authenticationService.Logout();

        if (!result.Succeeded)
        {
            dispatcher.Dispatch(new LogoutFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(result.Error!.ToApplicationError()));
        }

        dispatcher.Dispatch(new LogoutSucceededAction());
        dispatcher.Dispatch(new GoAction(Routes.Identity.Login(), true));
    }
}
