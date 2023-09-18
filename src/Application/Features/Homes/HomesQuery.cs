using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes;

public record HomesQuery : IRequest<ServiceResult<IEnumerable<Home>>>;

public class HomesQueryHandler : IRequestHandler<HomesQuery, ServiceResult<IEnumerable<Home>>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public HomesQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<Home>>> Handle(HomesQuery request, CancellationToken cancellationToken)
    {
        var homes = await _db.Homes
            .ProjectTo<Home>(_mapper.ConfigurationProvider)
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);

        return homes;
    }
}
