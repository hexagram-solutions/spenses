using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spenses.Client.Web.Store.Members;
using Spenses.Shared.Models.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class MembersCard
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    private IState<MembersState> MembersState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    [Inject]
    public IMessageService MessageService { get; set; } = null!;

    private IEnumerable<Member> Members => MembersState.Value.Members;

    public bool IsTotalHomeSplitPercentageValid
    {
        get
        {
            var totalHomeSplitPercentages = MembersState.Value.Members
                .Where(m => m.IsActive)
                .Sum(x => x.DefaultSplitPercentage);

            return totalHomeSplitPercentages == 1m;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<MemberCreationSucceededAction>(async _ =>
        {
            await ModalService.Hide();
        });

        SubscribeToAction<MemberUpdateSucceededAction>(async _ =>
        {
            await ModalService.Hide();
        });

        Dispatcher.Dispatch(new MembersRequestedAction(HomeId));
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
            $"Are you sure you want to remove {member.Name} from this home?");

        if (!confirmed)
            return;

        Dispatcher.Dispatch(new MemberDeletedAction(HomeId, member.Id));
    }

    private void OnActivateClicked(MouseEventArgs _, Guid memberId)
    {
        Dispatcher.Dispatch(new MemberActivatedAction(HomeId, memberId));
    }
}
