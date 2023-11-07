using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Features.Homes;
using Spenses.Client.Web.Features.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class EditMemberModal
{
    [Parameter]
    public Guid MemberId { get; set; }

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private MemberForm MemberFormRef { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private MembersState MembersState => GetState<MembersState>();

    private Member Member => MembersState.CurrentMember ?? new Member();

    protected override async Task OnInitializedAsync()
    {
        await Mediator.Send(new MembersState.MemberSelected(Home.Id, MemberId));

        // Direct mapping to new object ensure correct type is passed to validator
        //var currentMember = MembersState.CurrentMember!;

        //Member = new MemberProperties
        //{
        //    Name = currentMember.Name,
        //    ContactEmail = currentMember.ContactEmail,
        //    DefaultSplitPercentage = currentMember.DefaultSplitPercentage
        //};

        //Member = MembersState.CurrentMember!;

        await base.OnInitializedAsync();
    }

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await MemberFormRef.Validations.ValidateAll())
            return;

        await Mediator.Send(new MembersState.MemberUpdated(Home.Id, MemberId, Member));

        await Close();
    }
}
