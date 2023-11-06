using BlazorState;
using Spenses.Application.Models.Members;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberUpdated(Guid HomeId, Guid MemberId, MemberProperties Props) : IAction;

    public class MemberUpdatedHandler : ActionHandler<MemberUpdated>
    {
        private readonly IMembersApi _members;

        public MemberUpdatedHandler(IStore aStore, IMembersApi members)
            : base(aStore)
        {
            _members = members;
        }

        private MembersState MembersState => Store.GetState<MembersState>();

        public override async Task Handle(MemberUpdated aAction, CancellationToken aCancellationToken)
        {
            MembersState.MemberUpdating = true;

            var (homeId, memberId, props) = aAction;

            var response = await _members.PutMember(homeId, memberId, props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            MembersState.MemberUpdating = false;
        }
    }
}
