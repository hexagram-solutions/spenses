using System.ComponentModel.DataAnnotations;
using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Spenses.App.Components.Me;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Identity;
using Spenses.Utilities.Security;

namespace Spenses.App.Pages.Identity;

public partial class EmailVerificationRequired
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [SupplyParameterFromQuery]
    public string? Email { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    private IdentityState State => IdentityState.Value;

    private bool VerificationEmailReSent { get; set; }

    private readonly InputModel _model = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var currentUser = (await AuthenticationState).User;

        if (currentUser.FindFirst(ApplicationClaimTypes.EmailVerified)?.Value == true.ToString())
        {
            Dispatcher.Dispatch(new GoAction(Routes.Root));

            return;
        }

        SubscribeToAction<ResendVerificationEmailSucceededAction>(_ => { VerificationEmailReSent = true; });
    }

    private void ResendVerificationEmail(MouseEventArgs _, string email)
    {
        Dispatcher.Dispatch(new ResendVerificationEmailRequestedAction(email));
    }

    private void Submit()
    {
        Dispatcher.Dispatch(new ResendVerificationEmailRequestedAction(_model.Email));
    }

    private class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
