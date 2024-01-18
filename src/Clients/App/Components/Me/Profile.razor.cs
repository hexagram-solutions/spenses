using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Store.Me;
using Spenses.Shared.Models.Me;
using Spenses.Shared.Validators.Me;

namespace Spenses.App.Components.Me;

public partial class Profile
{
    [Parameter]
    public UserProfileProperties Properties { get; set; } = new();

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private IState<MeState> MeState { get; set; } = null!;

    private MudForm FormRef { get; set; } = null!;

    private readonly UserProfilePropertiesValidator _validator = new();

    private bool IsDirty => Properties.DisplayName != MeState.Value.CurrentUser?.DisplayName;

    private bool? UpdateSucceeded { get; set; }

    private bool UpdateEnabled => IsDirty && !MeState.Value.CurrentUserUpdating;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

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
