using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Members;

public record MemberQuery(Guid Id) : IRequest<ServiceResult<Member>>;

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
        var member = await _db.Members
            .ProjectTo<Member>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        return member is null ? new NotFoundErrorResult(request.Id) : member;
    }
}
