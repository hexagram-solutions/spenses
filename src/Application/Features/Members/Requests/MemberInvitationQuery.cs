using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Application.Features.Members.Requests;

public record MemberInvitationQuery(Guid HomeId, Guid MemberId, Guid InvitationId) : IAuthorizedRequest<Invitation>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class MemberInvitationQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<MemberInvitationQuery, Invitation>
{
    public async Task<Invitation> Handle(MemberInvitationQuery request, CancellationToken cancellationToken)
    {
        var (homeId, memberId, invitationId) = request;

        var invitation = await db.Invitations
            .Include(i => i.Member)
            .FirstOrDefaultAsync(i => i.Id == invitationId &&
                    i.MemberId == memberId &&
                    i.Member.HomeId == homeId,
                cancellationToken);

        if (invitation is null)
            throw new ResourceNotFoundException(invitationId);

        return mapper.Map<Invitation>(invitation)!;
    }
}
