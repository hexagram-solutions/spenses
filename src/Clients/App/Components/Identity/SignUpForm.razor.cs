using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
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

    private void Register(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        if (!string.IsNullOrEmpty(InvitationToken))
            RegisterRequest.InvitationToken = InvitationToken;

        Dispatcher.Dispatch(new RegistrationRequestedAction(RegisterRequest));
    }
}
