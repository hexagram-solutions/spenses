using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Identity;

namespace Spenses.App.Components.Identity;

public partial class VerifyEmail
{
    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool? Succeeded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (UserId is null || Code is null)
        {
            Dispatcher.Dispatch(new GoAction(Routes.Root));

            return;
        }

        Dispatcher.Dispatch(new EmailVerificationRequestedAction(UserId, Code));

        SubscribeToAction<EmailVerificationFailedAction>(_ => Succeeded = false);

        SubscribeToAction<EmailVerificationSucceededAction>(_ =>
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
}
