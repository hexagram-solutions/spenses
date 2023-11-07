using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Features.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class MembersTable
{
    [Parameter]
    public Guid HomeId { get; set; }

    [Inject]
    public IModalService ModalService { get; set; } = null!;

    [Inject]
    public IMessageService MessageService { get; set; } = null!;

    private MembersState MembersState => GetState<MembersState>();

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
            $"{member.Name} will be permanently removed from this home. This can't be undone.",
            "Are you sure you want to delete this member?");

        if (!confirmed)
            return;

        await Mediator.Send(new MembersState.MemberDeleted(HomeId, member.Id));
    }
}
