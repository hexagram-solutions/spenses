using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Spenses.App.Components.Shared;
using Spenses.App.Store.Homes;
using Spenses.App.Store.Members;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Components.Members;

public partial class EditMemberDialog
{
    [Parameter]
    [EditorRequired]
    public Guid MemberId { get; set; }

    [CascadingParameter]
    private MudDialogInstance Dialog { get; set; } = null!;

    [Inject]
    private IState<MembersState> MembersState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    public Member Member { get; set; } = new();

    private CustomValidations CustomValidationsRef { get; set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<MemberReceivedAction>(a => Member = a.Member);
        SubscribeToAction<MemberUpdateFailedAction>(a => CustomValidationsRef.AddErrors(a.Errors));

        Dispatcher.Dispatch(new MemberRequestedAction(Home.Id, MemberId));
    }

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save()
    {
        Dispatcher.Dispatch(new MemberUpdatedAction(Home.Id, MemberId, Member));
    }
}
