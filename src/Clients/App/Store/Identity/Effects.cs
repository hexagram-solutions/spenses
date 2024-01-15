using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Refit;
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

        if (result.Content!.Succeeded)
        {
            dispatcher.Dispatch(new LoginSucceededAction());
            dispatcher.Dispatch(new GoAction(action.ReturnUrl ?? Routes.Root));
        }
        else if (result.Content.RequiresTwoFactor)
        {
            dispatcher.Dispatch(new GoAction(Routes.Identity.TwoFactorLogin(action.ReturnUrl)));
        }
        else if (result.Content.IsNotAllowed)
        {
            dispatcher.Dispatch(new LoginFailedAction(IdentityErrors.Login.EmailVerificationRequired));
        }
        else if (result.Content.IsLockedOut)
        {
            dispatcher.Dispatch(new LoginFailedAction(IdentityErrors.Login.LockedOut));
        }
        else
        {
            dispatcher.Dispatch(new LoginFailedAction(IdentityErrors.Login.InvalidCredentials));
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

        if (result.Error!.Errors.ContainsKey(IdentityErrors.Register.DuplicateUserName) ||
            result.Error.Errors.ContainsKey(IdentityErrors.Register.DuplicateEmail))
        {
            dispatcher.Dispatch(new RegistrationFailedAction([IdentityErrors.Register.DuplicateEmail]));

            return;
        }

        if (result.Error!.Errors.ContainsKey(IdentityErrors.Password.PasswordTooShort))
        {
            dispatcher.Dispatch(new RegistrationFailedAction([IdentityErrors.Password.PasswordTooShort]));

            return;
        }

        if (result.Error!.Errors.ContainsKey(IdentityErrors.Password.EmailAsPassword) ||
            result.Error!.Errors.ContainsKey(IdentityErrors.Password.UserNameAsPassword))
        {
            dispatcher.Dispatch(new RegistrationFailedAction([IdentityErrors.Password.EmailAsPassword]));

            return;
        }

        if (result.Error!.Errors.ContainsKey(IdentityErrors.Password.PwnedPassword))
        {
            dispatcher.Dispatch(new RegistrationFailedAction([IdentityErrors.Password.PwnedPassword]));

            return;
        }

        if (result.Error.Errors.Count != 0)
        {
            dispatcher.Dispatch(new RegistrationFailedAction(result.Error.Errors.SelectMany(e => e.Value).ToArray()));

            return;
        }

        dispatcher.Dispatch(new RegistrationFailedAction(["An unknown error occurred."]));
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

            return;
        }

        dispatcher.Dispatch(new ResendVerificationEmailSucceededAction());
    }

    [EffectMethod]
    public async Task HandleLogoutRequestedAction(LogoutRequestedAction _, IDispatcher dispatcher)
    {
        if (await authenticationService.CheckAuthenticatedAsync())
        {
            var result = await authenticationService.Logout();

            if (!result.Succeeded)
            {
                dispatcher.Dispatch(new LogoutFailedAction());
                dispatcher.Dispatch(new ApplicationErrorAction(result.Error!.ToApplicationError()));

                return;
            }
        }

        dispatcher.Dispatch(new LogoutSucceededAction());
        dispatcher.Dispatch(new GoAction(Routes.Identity.Login(), true));
    }

    [EffectMethod]
    public async Task HandleForgotPasswordRequestedAction(ForgotPasswordRequestedAction action, IDispatcher dispatcher)
    {
        var response = await identityApi.ForgotPassword(new ForgotPasswordRequest { Email = action.Email });

        if (!response.IsSuccessStatusCode)
        {
            dispatcher.Dispatch(new ForgotPasswordFailedAction());

            return;
        }

        dispatcher.Dispatch(new ForgotPasswordSucceededAction());
    }

    [EffectMethod]
    public async Task HandleResetPasswordRequestedAction(ResetPasswordRequestedAction action, IDispatcher dispatcher)
    {
        var response = await identityApi.ResetPassword(action.Request);

        if (response.IsSuccessStatusCode)
        {
            dispatcher.Dispatch(new ResetPasswordSucceededAction());

            return;
        }

        var result = await response.Error.GetContentAsAsync<ProblemDetails>();

        if (result!.Errors.ContainsKey(IdentityErrors.Password.PasswordTooShort))
        {
            dispatcher.Dispatch(new ResetPasswordFailedAction([IdentityErrors.Password.PasswordTooShort]));

            return;
        }

        if (result.Errors.ContainsKey(IdentityErrors.Password.EmailAsPassword) ||
            result.Errors.ContainsKey(IdentityErrors.Password.UserNameAsPassword))
        {
            dispatcher.Dispatch(new ResetPasswordFailedAction([IdentityErrors.Password.EmailAsPassword]));

            return;
        }

        if (result.Errors.ContainsKey(IdentityErrors.Password.PwnedPassword))
        {
            dispatcher.Dispatch(new ResetPasswordFailedAction([IdentityErrors.Password.PwnedPassword]));

            return;
        }

        if (result.Errors.Count != 0)
        {
            dispatcher.Dispatch(new RegistrationFailedAction(result.Errors.SelectMany(e => e.Value).ToArray()));

            return;
        }

        dispatcher.Dispatch(new RegistrationFailedAction(["An unknown error occurred."]));
    }
}
