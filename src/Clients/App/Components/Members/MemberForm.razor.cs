using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.App.Store.Homes;
using Spenses.App.Store.Members;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Components.Members;

public partial class MemberForm
{
    [Parameter]
    public Guid? MemberId { get; set; }

    [Parameter]
    [EditorRequired]
    public MemberProperties Member { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private decimal DefaultSplitPercentage
    {
        get => Member.DefaultSplitPercentage * 100;
        set
        {
            Member.DefaultSplitPercentage = value / 100;
            CheckTotalSplitPercentage();
        }
    }

    private bool IsTotalHomeSplitPercentageValid { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        CheckTotalSplitPercentage();
    }

    private void CheckTotalSplitPercentage()
    {
        var otherHomeMembers = Home.Members
            .Where(x => x.Id != MemberId)
            .ToList();

        var totalHomeSplitPercentages = otherHomeMembers
            .Where(m => m.Status is MemberStatus.Active or MemberStatus.Invited)
            .Sum(x => x.DefaultSplitPercentage) + Member.DefaultSplitPercentage;

        IsTotalHomeSplitPercentageValid = totalHomeSplitPercentages == 1m;
    }
}
