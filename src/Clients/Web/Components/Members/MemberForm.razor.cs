using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Store.Homes;

namespace Spenses.Client.Web.Components.Members;

public partial class MemberForm
{
    [Parameter]
    public Member Member { get; set; } = new();

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    public Validations Validations { get; set; } = null!;

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

    protected override Task OnInitializedAsync()
    {
        CheckTotalSplitPercentage();

        return base.OnInitializedAsync();
    }

    private bool IsTotalHomeSplitPercentageValid { get; set; }

    // TODO: Good candidate for a reactive model
    private void CheckTotalSplitPercentage()
    {
        var otherHomeMembers = Home.Members
            .Where(x => x.Id != Member.Id)
            .ToList();

        var totalHomeSplitPercentages = otherHomeMembers
            .Where(m => m.IsActive)
            .Sum(x => x.DefaultSplitPercentage) + Member.DefaultSplitPercentage;

        IsTotalHomeSplitPercentageValid = totalHomeSplitPercentages == 1m;
    }
}
