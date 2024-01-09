using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using Spenses.App.Authentication;
using Spenses.App.Infrastructure;
using Spenses.Shared.Models.Authentication;

namespace Spenses.App.Components.Identity;

public partial class Login
{
    [Inject]
    private IAuthenticationService AuthenticationService { get; set; } = null!;

    [Inject]
    public required AuthenticationStateProvider AuthenticationState { get; set; }

    [Inject]
    private NavigationManager Navigation { get; set; } = null!; // todo: dispatch navigation action instead?

    [SupplyParameterFromQuery]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    private string ReturnUrl { get; set; } = Routes.Root;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authenticationState = await AuthenticationState.GetAuthenticationStateAsync();

        if (authenticationState.User.Identity?.IsAuthenticated == true)
            Navigation.NavigateTo(ReturnUrl);
    }

    private LoginRequest LoginRequest { get; } = new() { Email = string.Empty, Password = string.Empty };

    private string? ErrorMessage { get; set; }

    public async Task LogIn(EditContext editContext)
    {
        //if (await _fluentValidationValidator!.ValidateAsync())
        //    return;

        if (!editContext.ValidateObjectTree())
            return;

        var result = await AuthenticationService.Login(LoginRequest);

        if (result.Content!.Succeeded)
        {
            Navigation.NavigateTo(ReturnUrl);
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
