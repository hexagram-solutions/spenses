using System.ComponentModel;
using BlazorState;
using MediatR;
using Spenses.Application.Models.Common;
using Spenses.Client.Http;

namespace Spenses.Client.Web.Features.Members;

public partial class MembersState
{
    public record MemberDeleted(Guid HomeId, Guid MemberId) : IAction;

    public class MemberDeletedHandler : ActionHandler<MemberDeleted>
    {
        private readonly IMembersApi _members;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public MemberDeletedHandler(IStore aStore, IMembersApi members, IMediator mediator,
            INotificationService notificationService)
            : base(aStore)
        {
            _members = members;
            _mediator = mediator;
            _notificationService = notificationService;
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

            var deletionResult = response.Content!;

            switch (deletionResult.Type)
            {
                case DeletionType.Deleted:
                    await _notificationService.Success($"{response.Content!.Model.Name} was deleted successfully.",
                        "Member deleted");
                    break;

                case DeletionType.Deactivated:
                    await _notificationService.Warning($"{response.Content!.Model.Name} had expenses and/or payments " +
                        "associated with them and was deactivated.",
                        "Member deactivated");
                    break;

                default:
                    throw new InvalidEnumArgumentException(
                        nameof(deletionResult.Type),
                        (int) deletionResult.Type,
                        typeof(DeletionType));
            }
            

            //TODO: Sagas
            await _mediator.Send(new MembersRequested(homeId), aCancellationToken);
        }
    }
}
