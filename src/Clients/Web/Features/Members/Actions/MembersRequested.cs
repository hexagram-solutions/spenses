using BlazorState;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MembersRequested(Guid HomeId) : IAction;

    public class MembersRequestedHandler : ActionHandler<MembersRequested>
    {
        private readonly IMembersApi _members;

        public MembersRequestedHandler(IStore aStore, IMembersApi members)
            : base(aStore)
        {
            _members = members;
        }

        private MembersState MemberState => Store.GetState<MembersState>();

        public override async Task Handle(MembersRequested aAction, CancellationToken aCancellationToken)
        {
            MemberState.MembersRequesting = true;

            var membersResponse = await _members.GetMembers(aAction.HomeId);

            if (!membersResponse.IsSuccessStatusCode)
                throw new NotImplementedException();

            MemberState.Members = membersResponse.Content;

            MemberState.MembersRequesting = false;
        }
    }
}
