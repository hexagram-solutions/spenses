using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Spenses.App.Store.Me;
using Spenses.App.Store.Members;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Components.Members;

public partial class MembersTable
{
    [CascadingParameter]
    public Guid? CurrentHomeId { get; set; }

    [Parameter]
    [EditorRequired]
    public Member[] Members { get; set; } = [];

    [Inject]
    private IState<MembersState> MembersState { get; set; } = null!;

    [Inject]
    private IState<MeState> MeState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; init; } = null!;

    private bool IsLoading => MembersState.Value.MembersRequesting ||
        MembersState.Value.MemberCreating ||
        MembersState.Value.MemberUpdating ||
        MembersState.Value.MemberDeleting ||
        MembersState.Value.MemberDeleting ||
        MembersState.Value.MemberActivating ||
        MembersState.Value.MemberInviting ||
        MembersState.Value.MemberInvitationsCancelling;

    private IDialogReference? EditDialog { get; set; }

    private IDialogReference? InvitationDialog { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<MemberUpdateSucceededAction>(_ => EditDialog?.Close());
        SubscribeToAction<MemberInvitationSucceededAction>(_ => InvitationDialog?.Close());
    }

    private async Task OnEditClicked(MouseEventArgs _, Guid memberId)
    {
        var parameters =
            new DialogParameters<EditMemberDialog> { { x => x.MemberId, memberId } };

        EditDialog = await DialogService.ShowAsync<EditMemberDialog>("Edit member", parameters);
    }

    private async Task OnRemoveClicked(MouseEventArgs _, Member member)
    {
        var confirmed = await DialogService.ShowMessageBox(
            $"Are you sure you want to remove {member.Name} from this home?",
            "If this member has no expenses or payments associated with them, they be permanently removed from " +
            "this home. Otherwise, the member will be be deactivated with any existing payments or expenses " +
            "remaining associated with the member.",
            yesText: "Remove member",
            cancelText: "Close");

        if (confirmed != true)
            return;

        Dispatcher.Dispatch(new MemberDeletedAction(CurrentHomeId.GetValueOrDefault(), member.Id));
    }

    private void OnReactivateClicked(MouseEventArgs _, Guid memberId)
    {
        Dispatcher.Dispatch(new MemberActivatedAction(CurrentHomeId.GetValueOrDefault(), memberId));
    }

    private async Task OnSendInvitationClicked(MouseEventArgs _, Member member)
    {
        var parameters = new DialogParameters<MemberInvitationDialog>
        {
            { x => x.HomeId, CurrentHomeId.GetValueOrDefault() },
            { x => x.Member, member }
        };

        InvitationDialog = await DialogService.ShowAsync<MemberInvitationDialog>("Send invitation", parameters);
    }

    private async Task CancelInvitation(MouseEventArgs _, Member member)
    {
        var confirmed = await DialogService.ShowMessageBox(
            $"Are you sure you want to cancel the invitation for {member.Name}?",
            "The person you invited will not be able to join this home unless you invite them again.",
            yesText: "Cancel invitation",
            cancelText: "Close");

        if (confirmed != true)
            return;

        Dispatcher.Dispatch(new MemberInvitationsCancelledAction(CurrentHomeId.GetValueOrDefault(), member.Id));
    }
}
