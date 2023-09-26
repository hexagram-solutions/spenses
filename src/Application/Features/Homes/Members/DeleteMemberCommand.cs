using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Members;

public record DeleteMemberCommand(Guid HomeId, Guid MemberId) : IRequest<ServiceResult>;

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, ServiceResult>
{
    private readonly ApplicationDbContext _db;

    public DeleteMemberCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ServiceResult> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await _db.Members
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == memberId, cancellationToken);

        if (member is null)
            return new NotFoundErrorResult(memberId);

        _db.Members.Remove(member);

        await _db.SaveChangesAsync(cancellationToken);

        return new SuccessResult();
    }
}
