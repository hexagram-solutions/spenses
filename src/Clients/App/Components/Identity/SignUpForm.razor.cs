using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.App.Store.Identity;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Components.Identity;

public partial class SignUpForm
{
    [Parameter]
    public string? InvitationToken { get; set; }

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<IdentityState> IdentityState { get; set; } = null!;

    private RegisterRequest RegisterRequest { get; } = new();

    private void Register()
    {
        if (!string.IsNullOrEmpty(InvitationToken))
            RegisterRequest.InvitationToken = InvitationToken;

        Dispatcher.Dispatch(new RegistrationRequestedAction(RegisterRequest));
    }
}
