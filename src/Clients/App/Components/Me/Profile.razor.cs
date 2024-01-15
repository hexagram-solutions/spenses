using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Store.Me;
using Spenses.Shared.Models.Me;
using Spenses.Shared.Validators.Me;

namespace Spenses.App.Components.Me;

public partial class Profile
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<MeState> MeState { get; set; } = null!;

    private MudForm FormRef { get; set; } = null!;

    private readonly UserProfilePropertiesValidator _validator = new();

    private UserProfileProperties Properties { get; set; } = new();

    private bool IsDirty => Properties.DisplayName != MeState.Value.CurrentUser?.DisplayName;

    private bool? UpdateSucceeded { get; set; }

    private bool UpdateEnabled => IsDirty && !MeState.Value.CurrentUserUpdating;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (MeState.Value.CurrentUser is null && !MeState.Value.CurrentUserRequesting)
        {
            Dispatcher.Dispatch(new CurrentUserRequestedAction());

            SubscribeToAction<CurrentUserReceivedAction>(action => Properties = new UserProfileProperties
            {
                DisplayName = action.CurrentUser.DisplayName
            });
        }
        else
        {
            Properties = new UserProfileProperties { DisplayName = MeState.Value.CurrentUser!.DisplayName };
        }

        SubscribeToAction<CurrentUserUpdateSucceededAction>(_ => UpdateSucceeded = true);
    }

    private async Task UpdateInformation()
    {
        await FormRef.Validate();

        if (!FormRef.IsValid)
            return;

        Dispatcher.Dispatch(new CurrentUserUpdatedAction(Properties));
    }
}
