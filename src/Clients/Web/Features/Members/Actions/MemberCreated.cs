using BlazorState;
using Spenses.Application.Models.Members;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberCreated(Guid HomeId, MemberProperties Props) : IAction;

    public class MemberCreatedHandler : ActionHandler<MemberCreated>
    {
        private readonly IMembersApi _members;

        public MemberCreatedHandler(IStore aStore, IMembersApi members)
            : base(aStore)
        {
            _members = members;
        }

        private MembersState MembersState => Store.GetState<MembersState>();

        public override async Task Handle(MemberCreated aAction, CancellationToken aCancellationToken)
        {
            MembersState.MemberCreating = true;

            var (homeId, props) = aAction;

            var response = await _members.PostMember(homeId, props);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            MembersState.MemberCreating = false;
        }
    }
}
