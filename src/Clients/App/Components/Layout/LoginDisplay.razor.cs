using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Spenses.App.Store.Identity;
using Spenses.Utilities.Security;

namespace Spenses.App.Components.Layout;

public partial class LoginDisplay
{
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    private string UserName { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        UserName = (await AuthenticationState).User.GetName();
    }

    public void LogOut()
    {
        Dispatcher.Dispatch(new LogoutRequestedAction());
    }
}
