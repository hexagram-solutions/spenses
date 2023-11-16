using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Homes;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Requests;

public record HomeQuery(Guid HomeId) : IAuthorizedRequest<Home>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class HomeQueryCommandHandler(ApplicationDbContext db, IMapper mapper) : IRequestHandler<HomeQuery, Home>
{
    public async Task<Home> Handle(HomeQuery request, CancellationToken cancellationToken)
    {
        var home = await db.Homes
            .ProjectTo<Home>(mapper.ConfigurationProvider)
            .FirstAsync(h => h.Id == request.HomeId, cancellationToken);

        return home;
    }
}
