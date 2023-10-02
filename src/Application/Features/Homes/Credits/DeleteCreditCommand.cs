using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Credits;

public record DeleteCreditCommand(Guid HomeId, Guid CreditId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteCreditCommandHandler : IRequestHandler<DeleteCreditCommand>
{
    private readonly ApplicationDbContext _db;

    public DeleteCreditCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteCreditCommand request, CancellationToken cancellationToken)
    {
        var (homeId, creditId) = request;

        var credit = await _db.Credits
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == creditId, cancellationToken);

        if (credit is null)
            throw new ResourceNotFoundException(creditId);

        _db.Credits.Remove(credit);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
