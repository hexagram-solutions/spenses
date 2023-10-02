using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Members;

public record DeleteMemberCommand(Guid HomeId, Guid MemberId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand>
{
    private readonly ApplicationDbContext _db;

    public DeleteMemberCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await _db.Members
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == memberId, cancellationToken);

        if (member is null)
            throw new ResourceNotFoundException(memberId);

        _db.Members.Remove(member);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
