using BlazorState;
using MediatR;
using Spenses.Application.Models.Members;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberCreated(Guid HomeId, MemberProperties Props) : IAction;

    public class MemberCreatedHandler : ActionHandler<MemberCreated>
    {
        private readonly IMembersApi _members;
        private readonly IMediator _mediator;

        public MemberCreatedHandler(IStore aStore, IMembersApi members, IMediator mediator)
            : base(aStore)
        {
            _members = members;
            _mediator = mediator;
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

            // todo: sagas or something?
            await _mediator.Send(new MembersRequested(homeId), aCancellationToken);
        }
    }
}
