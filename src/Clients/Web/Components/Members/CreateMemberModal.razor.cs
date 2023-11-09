using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Store.Homes;
using Spenses.Client.Web.Store.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class CreateMemberModal
{
    [Inject]
    private IState<MembersState> MembersState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private Member Member { get; } = new() { DefaultSplitPercentage = 0.5m };

    private MemberForm MemberFormRef { get; set; } = null!;

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await MemberFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new MemberCreatedAction(Home.Id, MemberFormRef.Member));

        await Close();
    }
}
