using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;

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
            .ProjectTo<Member>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == memberId, cancellationToken);

        return member ?? throw new ResourceNotFoundException(memberId);
    }
}
