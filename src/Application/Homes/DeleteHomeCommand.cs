using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Resources.Relational;

namespace Spenses.Application.Homes;
public record DeleteHomeCommand(Guid Id) : IRequest<ServiceResult>;

public class DeleteHomeCommandHandler : IRequestHandler<DeleteHomeCommand, ServiceResult>
{
    private readonly ApplicationDbContext _db;

    public DeleteHomeCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ServiceResult> Handle(DeleteHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes.FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (home is null)
            return new NotFoundErrorResult(request.Id);

        _db.Homes.Remove(home);

        await _db.SaveChangesAsync(cancellationToken);

        return new SuccessResult();
    }
}
