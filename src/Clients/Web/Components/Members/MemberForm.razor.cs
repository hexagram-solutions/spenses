using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class MemberForm
{
    [Parameter]
    public MemberProperties Member { get; set; } = new();

    public Validations Validations { get; set; } = null!;

    private decimal DefaultSplitPercentage
    {
        get => Member.DefaultSplitPercentage * 100;
        set => Member.DefaultSplitPercentage = value / 100;
    }
}
