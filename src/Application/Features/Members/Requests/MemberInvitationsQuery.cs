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

public record MemberInvitationsQuery(Guid HomeId, Guid MemberId) : IAuthorizedRequest<Invitation[]>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class MemberInvitationsQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<MemberInvitationsQuery, Invitation[]>
{
    public async Task<Invitation[]> Handle(MemberInvitationsQuery request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await db.Members
            .Include(i => i.Invitations)
            .FirstOrDefaultAsync(m => m.Id == memberId && m.HomeId == homeId, cancellationToken);

        if (member is null)
            throw new ResourceNotFoundException(memberId);

        return mapper.Map<Invitation[]>(member.Invitations.OrderBy(i => i.CreatedAt))!;
    }
}
