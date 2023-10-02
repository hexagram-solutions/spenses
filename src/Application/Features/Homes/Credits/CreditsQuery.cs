using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Credits;

public record CreditsQuery(Guid HomeId) : IAuthorizedRequest<IEnumerable<Credit>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreditsQueryHandler : IRequestHandler<CreditsQuery, IEnumerable<Credit>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreditsQueryHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Credit>> Handle(CreditsQuery request,
        CancellationToken cancellationToken)
    {
        var credits = await _db.Credits
            .Where(e => e.Home.Id == request.HomeId)
            .OrderByDescending(h => h.Date)
            .ProjectTo<Credit>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return credits;
    }
}
