using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Homes;

public record HomeQuery(Guid Id) : IRequest<ServiceResult<Home>>;

public class HomeQueryCommandHandler : IRequestHandler<HomeQuery, ServiceResult<Home>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public HomeQueryCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Home>> Handle(HomeQuery request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes
            .ProjectTo<Home>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        return home is null ? new NotFoundErrorResult(request.Id) : home;
    }
}
