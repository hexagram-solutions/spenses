using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Features.Homes;

namespace Spenses.Client.Web.Components.Members;

public partial class MemberForm
{
    [Parameter]
    public Member Member { get; set; } = new();

    public Validations Validations { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

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

        var totalHomeSplitPercentages =
            otherHomeMembers.Sum(x => x.DefaultSplitPercentage) + Member.DefaultSplitPercentage;

        IsTotalHomeSplitPercentageValid = totalHomeSplitPercentages == 1m;
    }
}
