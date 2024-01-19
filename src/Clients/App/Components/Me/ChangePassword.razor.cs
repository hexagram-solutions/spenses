using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using Spenses.App.Store.Me;
using Spenses.Shared.Models.Me;

namespace Spenses.App.Components.Me;

public partial class ChangePassword
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<MeState> MeState { get; set; } = null!;

    public ChangePasswordRequest Request { get; set; } = new();

    private bool IsEditing { get; set; }

    private bool? Succeeded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        SubscribeToAction<ChangePasswordFailedAction>(_ => Succeeded = false);
        SubscribeToAction<ChangePasswordSucceededAction>(_ => Succeeded = true);
    }

    private void SubmitChangePassword(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new ChangePasswordRequestedAction(Request));
    }
}
