using Refit;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Client.Http;

public interface IInvitationsApi
{
    [Patch("/invitations/{invitationId}")]
    Task<IApiResponse<Invitation>> AcceptInvitation(Guid invitationId);

    [Delete("/invitations/{invitationId}")]
    Task<IApiResponse> DeclineInvitation(Guid invitationId);
}
