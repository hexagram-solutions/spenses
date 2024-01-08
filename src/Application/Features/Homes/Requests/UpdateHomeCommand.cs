using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Homes;

namespace Spenses.Application.Features.Homes.Requests;

public record UpdateHomeCommand(Guid HomeId, HomeProperties Props) : IAuthorizedRequest<Home>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateHomeCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<UpdateHomeCommand, Home>
{
    public async Task<Home> Handle(UpdateHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await db.Homes
            .Include(h => h.Members)
                .ThenInclude(m => m.User)
            .Include(h => h.CreatedBy)
            .Include(h => h.ModifiedBy)
            .FirstOrDefaultAsync(h => h.Id == request.HomeId, cancellationToken);

        mapper.Map(request.Props, home);

        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<Home>(home);
    }
}
