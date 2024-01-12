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

public partial class SignUp
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    [Inject]
    public required ILogger<SignUp> Logger { get; set; }

    private MudForm FormRef { get; set; } = null!;

    private readonly RegisterRequestValidator _validator = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ((await AuthenticationState).User.Identity?.IsAuthenticated == true)
            Dispatcher.Dispatch(new GoAction(Routes.Root));
    }

    private RegisterRequest RegisterRequest { get; } = new();

    private async Task Register()
    {
        await FormRef.Validate();

        if (!FormRef.IsValid)
            return;

        Dispatcher.Dispatch(new RegistrationRequestedAction(RegisterRequest));
    }
}
