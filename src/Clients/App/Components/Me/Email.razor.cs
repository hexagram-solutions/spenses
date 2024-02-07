using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Store.Me;
using Spenses.Shared.Models.Me;

namespace Spenses.App.Components.Me;

public partial class Email
{
    [Parameter]
    public ChangeEmailRequest Request { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<MeState> MeState { get; set; } = null!;

    private MudTextField<string> EmailTextFieldRef { get; set; } = null!;

    private bool IsEditing { get; set; }

    private bool IsDirty => Request.NewEmail != MeState.Value.CurrentUser?.Email;

    private bool ChangeEmailEnabled => IsDirty && !MeState.Value.ChangeEmailRequesting;

    private bool VerificationEmailSent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        SubscribeToAction<ChangeEmailSucceededAction>(_ => VerificationEmailSent = true);
    }

    private async Task ToggleEditMode()
    {
        Request.NewEmail = MeState.Value.CurrentUser!.Email;

        IsEditing = !IsEditing;

        if (IsEditing)
            await EmailTextFieldRef.FocusAsync();
    }

    private void ChangeEmail()
    {
        Dispatcher.Dispatch(new ChangeEmailRequestedAction(Request));
    }
}
