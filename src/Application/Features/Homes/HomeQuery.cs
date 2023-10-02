using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes;

public record HomeQuery(Guid HomeId) : IAuthorizedRequest<ServiceResult<Home>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult<Home> OnUnauthorized() => new UnauthorizedErrorResult();
}

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
            .FirstOrDefaultAsync(h => h.Id == request.HomeId, cancellationToken);

        if (home is null)
            return new NotFoundErrorResult(request.HomeId);

        return home;
    }
}
