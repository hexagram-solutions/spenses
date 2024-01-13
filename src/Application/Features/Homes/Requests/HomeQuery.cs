using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Homes;

namespace Spenses.Application.Features.Homes.Requests;

public record HomeQuery(Guid HomeId) : IAuthorizedRequest<Home>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class HomeQueryHandler(ApplicationDbContext db, IMapper mapper) : IRequestHandler<HomeQuery, Home>
{
    public async Task<Home> Handle(HomeQuery request, CancellationToken cancellationToken)
    {
        var home = await db.Homes
            .ProjectTo<Home>(mapper.ConfigurationProvider)
            .FirstAsync(h => h.Id == request.HomeId, cancellationToken);

        return home;
    }
}
