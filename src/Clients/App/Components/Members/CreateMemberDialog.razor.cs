using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Spenses.App.Infrastructure;
using Spenses.App.Store.Homes;
using Spenses.App.Store.Members;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Models.Members;

namespace Spenses.App.Components.Members;

public partial class CreateMemberDialog
{
    [CascadingParameter]
    private MudDialogInstance Dialog { get; set; } = null!;

    [Inject]
    private IState<MembersState> MembersState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    public CreateMemberProperties Member { get; set; } = new();

    private EditForm EditFormRef { get; set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SubscribeToAction<MemberCreationFailedAction>(a => EditFormRef.AddValidationErrors(a.Errors));
    }

    private void Close()
    {
        Dialog.Cancel();
    }

    private void Save()
    {
        Dispatcher.Dispatch(new MemberCreatedAction(Home.Id, Member));
    }
}
