using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.App.Store.Identity;
using Spenses.App.Store.Me;

namespace Spenses.App.Components.Me;

public partial class LoginDisplay
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    [Inject]
    private IState<MeState> MeState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (MeState.Value.CurrentUser is null && !MeState.Value.CurrentUserRequesting)
            Dispatcher.Dispatch(new CurrentUserRequestedAction());
    }

    public void LogOut()
    {
        Dispatcher.Dispatch(new LogoutRequestedAction());
    }
}
