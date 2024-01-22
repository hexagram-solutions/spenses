using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Pages.Identity;

public partial class ForgotPassword
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    private ForgotPasswordRequest Request { get; set; } = new();

    private bool? Succeeded { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<ForgotPasswordSucceededAction>(_ => Succeeded = true);
        SubscribeToAction<ForgotPasswordFailedAction>(_ => Succeeded = false);
    }

    private void RequestPasswordReset(EditContext editContext)
    {
        Succeeded = null;

        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new ForgotPasswordRequestedAction(Request.Email));
    }
}
