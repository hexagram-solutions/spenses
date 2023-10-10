using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Requests;

public record DeleteHomeCommand(Guid HomeId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteHomeCommandHandler : IRequestHandler<DeleteHomeCommand>
{
    private readonly ApplicationDbContext _db;

    public DeleteHomeCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes.FirstAsync(h => h.Id == request.HomeId, cancellationToken);

        _db.Homes.Remove(home);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
