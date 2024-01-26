using Microsoft.AspNetCore.Authorization;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Application.Features.Members.Requests;

public record CancelInvitationCommand(Guid HomeId, Guid MemberId, Guid InvitationId) : IAuthorizedRequest<Invitation>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}
