using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using Spenses.App.Authentication;
using Spenses.App.Infrastructure;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Components.Identity;

public partial class Login
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    private IAuthenticationService AuthenticationService { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!; // todo: dispatch navigation action instead?

    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ((await AuthenticationState).User.Identity?.IsAuthenticated == true)
            Navigation.NavigateTo(ReturnUrl ?? Routes.Root);
    }

    private LoginRequest LoginRequest { get; } = new() { Email = string.Empty, Password = string.Empty };

    private string? ErrorMessage { get; set; }

    public async Task LogIn(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        var result = await AuthenticationService.Login(LoginRequest);

        if (result.Content!.Succeeded)
        {
            Navigation.NavigateTo(ReturnUrl ?? Routes.Root);
        }
        else if (result.Content!.RequiresTwoFactor)
        {
            Navigation.NavigateTo(Routes.Identity.TwoFactorLogin(ReturnUrl));
        }
        else if (result.Content.IsLockedOut)
        {
            ErrorMessage = "This account is locked.";
        }
        else
        {
            ErrorMessage = "Your email or password was incorrect. Please try again.";
        }
    }
}
