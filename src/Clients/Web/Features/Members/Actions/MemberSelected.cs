using BlazorState;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberSelected(Guid HomeId, Guid MemberId) : IAction;

    public class MemberSelectedHandler : ActionHandler<MemberSelected>
    {
        private readonly IMembersApi _members;

        public MemberSelectedHandler(IStore aStore, IMembersApi members)
            : base(aStore)
        {
            _members = members;
        }

        private MembersState MembersState => Store.GetState<MembersState>();

        public override async Task Handle(MemberSelected aAction, CancellationToken aCancellationToken)
        {
            MembersState.MemberRequesting = true;

            var (homeId, memberId) = aAction;

            var response = await _members.GetMember(homeId, memberId);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            MembersState.CurrentMember = response.Content;

            MembersState.MemberRequesting = false;
        }
    }
}
