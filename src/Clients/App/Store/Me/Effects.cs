using Fluxor;
using Refit;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Shared;
using Spenses.Client.Http;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Store.Me;

public class Effects(IMeApi me)
{
    [EffectMethod]
    public async Task HandleCurrentUserRequested(CurrentUserRequestedAction _, IDispatcher dispatcher)
    {
        var response = await me.GetMe();

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new CurrentUserRequestFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new CurrentUserReceivedAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleCurrentUserUpdated(CurrentUserUpdatedAction action, IDispatcher dispatcher)
    {
        var response = await me.UpdateMe(action.Props);

        if (response.Error is not null)
        {
            dispatcher.Dispatch(new CurrentUserUpdateFailedAction());
            dispatcher.Dispatch(new ApplicationErrorAction(response.Error.ToApplicationError()));

            return;
        }

        dispatcher.Dispatch(new CurrentUserUpdateSucceededAction(response.Content!));
    }

    [EffectMethod]
    public async Task HandleChangeEmailRequested(ChangeEmailRequestedAction action, IDispatcher dispatcher)
    {
        var result = await me.ChangeEmail(action.Request);

        if (result.IsSuccessStatusCode)
        {
            dispatcher.Dispatch(new ChangeEmailSucceededAction());

            return;
        }

        var problemDetails = await result.Error.GetContentAsAsync<ProblemDetails>();

        if (problemDetails!.Errors.ContainsKey(IdentityErrors.Register.DuplicateUserName) ||
            problemDetails.Errors.ContainsKey(IdentityErrors.Register.DuplicateEmail))
        {
            dispatcher.Dispatch(new ChangeEmailFailedAction([IdentityErrors.Register.DuplicateEmail]));

            return;
        }

        if (problemDetails.Errors.Count != 0)
        {
            dispatcher.Dispatch(new ChangeEmailFailedAction(problemDetails.Errors.SelectMany(e => e.Value).ToArray()));

            return;
        }

        dispatcher.Dispatch(new ChangeEmailFailedAction(["An unknown error occurred when changing your email."]));
    }

    [EffectMethod]
    public async Task HandleChangePasswordRequested(ChangePasswordRequestedAction action, IDispatcher dispatcher)
    {
        var result = await me.ChangePassword(action.Request);

        if (result.IsSuccessStatusCode)
        {
            dispatcher.Dispatch(new ChangePasswordSucceededAction());

            return;
        }

        var problemDetails = await result.Error.GetContentAsAsync<ProblemDetails>();



        if (problemDetails!.Errors.ContainsKey(IdentityErrors.Password.PasswordMismatch))
        {
            dispatcher.Dispatch(new ChangePasswordFailedAction([IdentityErrors.Password.PasswordMismatch]));

            return;
        }

        if (problemDetails.Errors.ContainsKey(IdentityErrors.Password.PasswordTooShort))
        {
            dispatcher.Dispatch(new ChangePasswordFailedAction([IdentityErrors.Password.PasswordTooShort]));

            return;
        }

        if (problemDetails.Errors.ContainsKey(IdentityErrors.Password.EmailAsPassword) ||
            problemDetails.Errors.ContainsKey(IdentityErrors.Password.UserNameAsPassword))
        {
            dispatcher.Dispatch(new ChangePasswordFailedAction([IdentityErrors.Password.EmailAsPassword]));

            return;
        }

        if (problemDetails.Errors.ContainsKey(IdentityErrors.Password.PwnedPassword))
        {
            dispatcher.Dispatch(new ChangePasswordFailedAction([IdentityErrors.Password.PwnedPassword]));

            return;
        }

        if (problemDetails.Errors.Count != 0)
        {
            dispatcher.Dispatch(new ChangePasswordFailedAction(problemDetails.Errors.SelectMany(e => e.Value).ToArray()));

            return;
        }

        dispatcher.Dispatch(new ChangePasswordFailedAction(["An unknown error occurred when changing your password."]));
    }
}
