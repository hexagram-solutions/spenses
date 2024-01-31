using Refit;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Client.Http;

public interface IInvitationsApi
{
    [Patch("/invitations")]
    Task<IApiResponse<Invitation>> AcceptInvitation(Guid invitationId);

    [Delete("/invitations")]
    Task<IApiResponse> DeclineInvitation(Guid invitationId);
}
