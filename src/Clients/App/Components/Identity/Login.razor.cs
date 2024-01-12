using Fluxor;
using Fluxor.Blazor.Web.Middlewares.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

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

    private MudForm FormRef { get; set; } = null!;

    private readonly LoginRequestValidator _validator = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ((await AuthenticationState).User.Identity?.IsAuthenticated == true)
            Dispatcher.Dispatch(new GoAction(ReturnUrl ?? Routes.Root));
    }

    private LoginRequest LoginRequest { get; } = new();

    public async Task LogIn()
    {
        await FormRef.Validate();

        if (!FormRef.IsValid)
            return;

        Dispatcher.Dispatch(new LoginRequestedAction(LoginRequest, ReturnUrl));
    }
}
