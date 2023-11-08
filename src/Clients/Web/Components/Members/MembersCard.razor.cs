using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Features.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class MembersCard
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    public IModalService ModalService { get; set; } = null!;

    [Inject]
    public IMessageService MessageService { get; set; } = null!;

    private MembersState MembersState => GetState<MembersState>();

    private IEnumerable<Member> Members => MembersState.Members;

    public bool IsTotalHomeSplitPercentageValid
    {
        get
        {
            var totalHomeSplitPercentages = MembersState.Members
                .Where(m => m.IsActive)
                .Sum(x => x.DefaultSplitPercentage);

            return totalHomeSplitPercentages == 1m;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Mediator.Send(new MembersState.MembersRequested(HomeId));

        await base.OnInitializedAsync();
    }

    private Task AddMember()
    {
        return ModalService.Show<CreateMemberModal>();
    }

    private Task OnEditClicked(MouseEventArgs _, Guid memberId)
    {
        return ModalService.Show<EditMemberModal>(p =>
        {
            p.Add(x => x.MemberId, memberId);
        });
    }

    private async Task OnDeleteClicked(MouseEventArgs _, Member member)
    {
        var confirmed = await MessageService.Confirm(
            "If this member has no expenses or payments associated with them, they be permanently removed from " +
            "this home. Otherwise, the member will be be deactivated with any existing payments or expenses " +
            "remaining associated with the member.",
            $"Are you sure you want to remove this {member.Name} from this home?");

        if (!confirmed)
            return;

        await Mediator.Send(new MembersState.MemberDeleted(HomeId, member.Id));
    }

    private Task OnActivateClicked(MouseEventArgs _, Guid memberId)
    {
        return Mediator.Send(new MembersState.MemberActivated(HomeId, memberId));
    }
}
