using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using Spenses.App.Authentication;
using Spenses.App.Infrastructure;
using Spenses.Shared.Models.Authentication;

namespace Spenses.App.Components.Identity;

public partial class SignUp
{
    [Inject]
    private IAuthenticationService AuthenticationService { get; set; } = null!;

    [Inject]
    public required AuthenticationStateProvider AuthenticationState { get; set; }

    [Inject]
    private NavigationManager Navigation { get; set; } = null!; // todo: dispatch navigation action instead?

    [Inject]
    public required ILogger<SignUp> Logger { get; set; }

    [SupplyParameterFromQuery]
    private string ReturnUrl { get; init; } = Routes.Root;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var authenticationState = await AuthenticationState.GetAuthenticationStateAsync();

        if (authenticationState.User.Identity?.IsAuthenticated == true)
            Navigation.NavigateTo(Routes.Root);
    }

    private List<string> Errors { get; } = [];

    private RegisterRequest RegisterRequest { get; } = new()
    {
        Email = string.Empty,
        Password = string.Empty
    };

    private async Task RegisterUser(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        var result = await AuthenticationService.Register(RegisterRequest);

        if (result.Succeeded)
        {
            Navigation.NavigateTo(ReturnUrl);
        }
        else if (result.Error!.Errors.ContainsKey(IdentityErrors.DuplicateUserName) ||
                 result.Error.Errors.ContainsKey(IdentityErrors.DuplicateEmail))
        {
            Errors.Add("It looks like you may already have an account with us. Use your credentials to log in " +
                "instead.");
        }
        else if (result.Error.Errors.Count != 0)
        {
            Errors.AddRange(result.Error.Errors.SelectMany(e => e.Value));
        }
        else
        {
            Errors.Add("An unknown error occurred.");

            Logger.LogError("Error when registering user: {Error}", result.Error.Detail);
        }
    }
}
