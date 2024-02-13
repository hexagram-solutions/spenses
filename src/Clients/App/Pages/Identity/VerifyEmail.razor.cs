using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Pages.Identity;

public partial class VerifyEmail
{
    [SupplyParameterFromQuery]
    private Guid? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    [SupplyParameterFromQuery]
    private string? NewEmail { get; set; }

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private bool? Succeeded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (!UserId.HasValue || Code is null)
        {
            Dispatcher.Dispatch(new GoAction(Routes.Root));

            return;
        }

        Dispatcher.Dispatch(new EmailVerificationRequestedAction(
            new VerifyEmailRequest(UserId.GetValueOrDefault(), Code, NewEmail)));

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

        Dispatcher.Dispatch(new GoAction(Routes.Identity.Login(), true));
    }
}
