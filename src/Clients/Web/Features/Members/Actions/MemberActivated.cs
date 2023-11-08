using BlazorState;
using MediatR;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberActivated(Guid HomeId, Guid MemberId) : IAction;

    public class MemberActivatedHandler : ActionHandler<MemberActivated>
    {
        private readonly IMembersApi _members;
        private readonly IMediator _mediator;

        public MemberActivatedHandler(IStore aStore, IMembersApi members, IMediator mediator)
            : base(aStore)
        {
            _members = members;
            _mediator = mediator;
        }

        private MembersState MembersState => Store.GetState<MembersState>();

        public override async Task Handle(MemberActivated aAction, CancellationToken aCancellationToken)
        {
            MembersState.MemberActivating = true;

            var (homeId, memberId) = aAction;

            var response = await _members.ActivateMember(homeId, memberId);

            if (!response.IsSuccessStatusCode)
                throw new NotImplementedException();

            MembersState.MemberActivating = false;

            //TODO: Sagas
            await _mediator.Send(new MembersRequested(homeId), aCancellationToken);
        }
    }
}
