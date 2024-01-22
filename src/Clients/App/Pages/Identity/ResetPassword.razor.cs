using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using MudBlazor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.App.Pages.Identity;

public partial class ResetPassword
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    [SupplyParameterFromQuery]
    private string? Email
    {
        get => Request.Email;
        set => Request.Email = value ?? string.Empty;
    }

    [SupplyParameterFromQuery]
    private string? Code
    {
        get => Request.ResetCode;
        set => Request.ResetCode = value ?? string.Empty;
    }

    private ResetPasswordRequest Request { get; set; } = new();

    private bool? Succeeded { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<ResetPasswordFailedAction>(_ => Succeeded = false);

        SubscribeToAction<ResetPasswordSucceededAction>(_ =>
        {
            Succeeded = true;

            NavigateToLogin();
        });
    }

    private async void NavigateToLogin()
    {
        await Task.Delay(5000);

        Dispatcher.Dispatch(new GoAction(Routes.Identity.Login()));
    }

    private void UpdatePassword(EditContext context)
    {
        if (!context.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new ResetPasswordRequestedAction(Request));
    }
}
