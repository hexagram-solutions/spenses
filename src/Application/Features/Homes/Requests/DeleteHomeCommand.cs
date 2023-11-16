using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Requests;

public record DeleteHomeCommand(Guid HomeId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteHomeCommandHandler(ApplicationDbContext db)
    : IRequestHandler<DeleteHomeCommand>
{
    public async Task Handle(DeleteHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await db.Homes.FirstAsync(h => h.Id == request.HomeId, cancellationToken);

        db.Homes.Remove(home);

        await db.SaveChangesAsync(cancellationToken);
    }
}
