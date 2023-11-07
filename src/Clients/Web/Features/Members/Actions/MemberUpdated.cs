using BlazorState;
using MediatR;
using Spenses.Application.Models.Members;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberUpdated(Guid HomeId, Guid MemberId, MemberProperties Props) : IAction;

    public class MemberUpdatedHandler : ActionHandler<MemberUpdated>
    {
        private readonly IMembersApi _members;
        private readonly IMediator _mediator;

        public MemberUpdatedHandler(IStore aStore, IMembersApi members, IMediator mediator)
            : base(aStore)
        {
            _members = members;
            _mediator = mediator;
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

            // todo: sagas or something?
            await _mediator.Send(new MembersRequested(homeId), aCancellationToken);
        }
    }
}
