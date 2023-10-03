using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Members.Requests;

public record MemberQuery(Guid HomeId, Guid MemberId) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class MemberQueryHandler : IRequestHandler<MemberQuery, Member>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public MemberQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Member> Handle(MemberQuery request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await _db.Members
            .Where(m => m.HomeId == homeId)
            .ProjectTo<Member>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == memberId, cancellationToken);

        return member ?? throw new ResourceNotFoundException(memberId);
    }
}
