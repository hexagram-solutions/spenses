using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Members;

public record MembersQuery(Guid HomeId) : IRequest<ServiceResult<IEnumerable<Member>>>;

public class MembersQueryHandler : IRequestHandler<MembersQuery, ServiceResult<IEnumerable<Member>>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public MembersQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<Member>>> Handle(MembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _db.Members
            .Where(m => m.HomeId == request.HomeId)
            .ProjectTo<Member>(_mapper.ConfigurationProvider)
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);

        return members;
    }
}
