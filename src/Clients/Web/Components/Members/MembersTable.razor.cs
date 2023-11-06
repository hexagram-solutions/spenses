using Microsoft.AspNetCore.Components;
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
}
