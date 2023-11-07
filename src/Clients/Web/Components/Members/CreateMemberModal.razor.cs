using Microsoft.AspNetCore.Components;
using Spenses.Application.Models.Homes;
using Spenses.Application.Models.Members;
using Spenses.Client.Web.Features.Homes;
using Spenses.Client.Web.Features.Members;

namespace Spenses.Client.Web.Components.Members;

public partial class CreateMemberModal
{
    private Member Member { get; } = new();

    [Inject]
    public IModalService ModalService { get; init; } = null!;

    private MemberForm MemberFormRef { get; set; } = null!;

    private Home Home => GetState<HomeState>().CurrentHome!;

    private MembersState MembersState => GetState<MembersState>();

    private Task Close()
    {
        return ModalService.Hide();
    }

    private async Task Save()
    {
        if (!await MemberFormRef.Validations.ValidateAll())
            return;

        await Mediator.Send(new MembersState.MemberCreated(Home.Id, Member));

        await Close();
    }
}
