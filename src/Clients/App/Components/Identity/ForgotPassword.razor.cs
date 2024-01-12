using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.App.Components.Identity;

public partial class ForgotPassword
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    private ForgotPasswordRequest Request { get; set; } = new();

    private bool? Succeeded { get; set; }

    private MudForm FormRef { get; set; } = null!;

    private readonly ForgotPasswordRequestValidator _validator = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<ForgotPasswordSucceededAction>(_ => Succeeded = true);
        SubscribeToAction<ForgotPasswordFailedAction>(_ => Succeeded = false);
    }

    private async Task RequestPasswordReset()
    {
        Succeeded = null;

        await FormRef.Validate();

        if (!FormRef.IsValid)
            return;

        Dispatcher.Dispatch(new ForgotPasswordRequestedAction(Request.Email));
    }
}
