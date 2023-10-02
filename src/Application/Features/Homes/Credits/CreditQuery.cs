using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Credits;

public record CreditQuery(Guid HomeId, Guid CreditId) : IAuthorizedRequest<Credit>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreditQueryCommandHandler : IRequestHandler<CreditQuery, Credit>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreditQueryCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Credit> Handle(CreditQuery request, CancellationToken cancellationToken)
    {
        var (homeId, creditId) = request;

        var credit = await _db.Credits
            .Where(e => e.Home.Id == homeId)
            .ProjectTo<Credit>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(h => h.Id == creditId, cancellationToken);

        return credit ?? throw new ResourceNotFoundException(creditId);
    }
}

