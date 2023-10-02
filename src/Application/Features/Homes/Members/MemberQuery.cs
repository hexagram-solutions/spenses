using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Members;

public record MemberQuery(Guid HomeId, Guid MemberId) : IAuthorizedRequest<ServiceResult<Member>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult<Member> OnUnauthorized() => new UnauthorizedErrorResult();
}

public class MemberQueryHandler : IRequestHandler<MemberQuery, ServiceResult<Member>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public MemberQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Member>> Handle(MemberQuery request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await _db.Members
            .Where(m => m.HomeId == homeId)
            .ProjectTo<Member>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == memberId, cancellationToken);

        return member is null ? new NotFoundErrorResult(request.MemberId) : member;
    }
}
