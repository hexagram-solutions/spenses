using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;
using Spenses.Utilities.Security;

namespace Spenses.App.Pages.Identity;

public partial class EmailVerificationRequired
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    private IdentityState State => IdentityState.Value;

    private bool VerificationEmailReSent { get; set; }

    public string Email { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var currentUser = (await AuthenticationState).User;

        if (currentUser.FindFirst(ApplicationClaimTypes.EmailVerified)?.Value == true.ToString())
        {
            Dispatcher.Dispatch(new GoAction(Routes.Root));

            return;
        }

        Email = currentUser.GetEmail();

        SubscribeToAction<ResendVerificationEmailSucceededAction>(_ => { VerificationEmailReSent = true; });
    }

    private void ResendVerificationEmail()
    {
        Dispatcher.Dispatch(new ResendVerificationEmailRequestedAction(Email));
    }
}
