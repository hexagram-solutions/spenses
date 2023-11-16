using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Members.Requests;

public record MembersQuery(Guid HomeId) : IAuthorizedRequest<IEnumerable<Member>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class MembersQueryHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<MembersQuery, IEnumerable<Member>>
{
    public async Task<IEnumerable<Member>> Handle(MembersQuery request, CancellationToken cancellationToken)
    {
        var members = await db.Members
            .Where(m => m.HomeId == request.HomeId)
            .OrderBy(m => m.Name)
            .ProjectTo<Member>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return members;
    }
}
