using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using Morris.Blazor.Validation.Extensions;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Components.Identity;

public partial class Login
{
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ((await AuthenticationState).User.Identity?.IsAuthenticated == true)
            Dispatcher.Dispatch(new GoAction(ReturnUrl ?? Routes.Root));
    }

    private LoginRequest LoginRequest { get; } = new() { Email = string.Empty, Password = string.Empty };

    public void LogIn(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new LoginRequestedAction(LoginRequest, ReturnUrl));
    }
}
