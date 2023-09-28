using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Authorization;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Features.Homes;

public record HomeQuery(Guid Id) : IRequest<ServiceResult<Home>>;

public class HomeQueryCommandHandler : IRequestHandler<HomeQuery, ServiceResult<Home>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserAuthorizationService _authorizationService;

    public HomeQueryCommandHandler(ApplicationDbContext db, IMapper mapper,
        ICurrentUserAuthorizationService authorizationService)
    {
        _db = db;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    public async Task<ServiceResult<Home>> Handle(HomeQuery request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes
            .ProjectTo<Home>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (home is null)
            return new NotFoundErrorResult(request.Id);

        var authorizationResult = await _authorizationService.AuthorizeAsync(home, new HomeMemberRequirement());

        if (!authorizationResult.Succeeded)
            return new NotFoundErrorResult(request.Id);

        return home;
    }
}
