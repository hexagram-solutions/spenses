using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Members;

namespace Spenses.Application.Features.Members.Requests;

public record MemberQuery(Guid HomeId, Guid MemberId) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class MemberQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<MemberQuery, Member>
{
    public async Task<Member> Handle(MemberQuery request, CancellationToken cancellationToken)
    {
        var (_, memberId) = request;

        var member = await db.Members
                .FirstOrDefaultAsync(h => h.Id == memberId, cancellationToken)
            ?? throw new ResourceNotFoundException(memberId);

        return mapper.Map<Member>(member);
    }
}
