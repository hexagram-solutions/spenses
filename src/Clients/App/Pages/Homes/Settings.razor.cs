using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Morris.Blazor.Validation.Extensions;
using Spenses.App.Store.Homes;
using Spenses.App.Store.Members;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Pages.Homes;

public partial class Settings
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IState<MembersState> MembersState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome ?? new Home();

    public bool IsTotalHomeSplitPercentageValid
    {
        get
        {
            var totalHomeSplitPercentages = MembersState.Value.Members
                .Where(m => m.Status is MemberStatus.Active or MemberStatus.Inactive)
                .Sum(x => x.DefaultSplitPercentage);

            return totalHomeSplitPercentages == 1m;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (HomesState.Value.CurrentHome?.Id == HomeId)
            return;

        Dispatcher.Dispatch(new HomeRequestedAction(HomeId));
    }
    private void Save(EditContext editContext)
    {
        if (!editContext.ValidateObjectTree())
            return;

        Dispatcher.Dispatch(new HomeUpdatedAction(HomeId, Home!));
    }
}
