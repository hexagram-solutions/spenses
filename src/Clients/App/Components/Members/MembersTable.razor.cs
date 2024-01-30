using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
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
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; init; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    private bool IsLoading => MembersState.Value.MembersRequesting ||
        MembersState.Value.MemberCreating ||
        MembersState.Value.MemberUpdating ||
        MembersState.Value.MemberDeleting ||
        MembersState.Value.MemberDeleting ||
        MembersState.Value.MemberActivating;

    private IDialogReference? EditDialog { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<MemberUpdateSucceededAction>(_ => EditDialog?.Close());
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
            "Remove member",
            cancelText: "Cancel");

        if (confirmed != true)
            return;

        Dispatcher.Dispatch(new MemberDeletedAction(CurrentHomeId.GetValueOrDefault(), member.Id));
    }

    private void OnReactivateClicked(MouseEventArgs _, Guid memberId)
    {
        Dispatcher.Dispatch(new MemberActivatedAction(CurrentHomeId.GetValueOrDefault(), memberId));
    }
}
