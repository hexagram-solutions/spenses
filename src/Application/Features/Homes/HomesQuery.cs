using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Features.Homes;

public record HomesQuery : IRequest<ServiceResult<IEnumerable<Home>>>;

public class HomesQueryHandler : IRequestHandler<HomesQuery, ServiceResult<IEnumerable<Home>>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public HomesQueryHandler(ApplicationDbContext db, IMapper mapper, ICurrentUserService currentUserService)
    {
        _db = db;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<ServiceResult<IEnumerable<Home>>> Handle(HomesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.CurrentUser.GetId();

        var homes = await _db.Homes
            .Where(h => h.Members.Select(m => m.UserId).Contains(currentUserId))
            .ProjectTo<Home>(_mapper.ConfigurationProvider)
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);

        return homes;
    }
}
