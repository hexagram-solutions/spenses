using BlazorState;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberDeleted(Guid HomeId, Guid MemberId) : IAction;

    public class MemberDeletedHandler : ActionHandler<MemberDeleted>
    {
        private readonly IMembersApi _members;

        public MemberDeletedHandler(IStore aStore, IMembersApi members)
            : base(aStore)
        {
            _members = members;
        }

        private MembersState MembersState => Store.GetState<MembersState>();

        public override async Task Handle(MemberDeleted aAction, CancellationToken aCancellationToken)
        {
            MembersState.MemberDeleting = true;

            var (homeId, memberId) = aAction;

            var response = await _members.DeleteMember(homeId, memberId);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            MembersState.MemberDeleting = false;
        }
    }
}
