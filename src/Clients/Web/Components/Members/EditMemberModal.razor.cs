using Fluxor;
using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Store.Homes;
using Spenses.Client.Web.Store.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class EditMemberModal
{
    [Parameter]
    public Guid MemberId { get; set; }

    [Inject]
    private IState<MembersState> MembersState { get; set; } = null!;

    [Inject]
    private IState<HomesState> HomesState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private Home Home => HomesState.Value.CurrentHome!;

    private MemberForm MemberFormRef { get; set; } = null!;

    private Member Member
    {
        get
        {
            var currentMember = MembersState.Value.CurrentMember;

            if (currentMember is null)
                return new Member { DefaultSplitPercentage = 0.5m };

            return new Member
            {
                Name = currentMember.Name,
                ContactEmail = currentMember.ContactEmail,
                DefaultSplitPercentage = currentMember.DefaultSplitPercentage
            };
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Dispatcher.Dispatch(new MemberRequestedAction(Home.Id, MemberId));
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await MemberFormRef.Validations.ValidateAll())
            return;

        Dispatcher.Dispatch(new MemberUpdatedAction(Home.Id, MemberId, MemberFormRef.Member));
    }
}
