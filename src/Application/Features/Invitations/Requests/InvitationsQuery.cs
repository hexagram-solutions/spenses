using Microsoft.AspNetCore.Authorization;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Application.Features.Invitations.Requests;

public record InvitationsQuery(Guid HomeId) : IAuthorizedRequest<Invitation>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}