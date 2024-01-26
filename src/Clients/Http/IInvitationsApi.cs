using Refit;

namespace Spenses.Client.Http;

public interface IInvitationsApi
{
    [Patch("/invitations")]
    Task<IApiResponse> AcceptInvitation(string token);
}
