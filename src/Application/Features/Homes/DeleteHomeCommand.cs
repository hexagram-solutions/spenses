using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Authorization;
using Spenses.Application.Common.Results;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes;

public record DeleteHomeCommand(Guid HomeId) : IAuthorizedRequest<ServiceResult>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult OnUnauthorized() => new UnauthorizedErrorResult();
}

public class DeleteHomeCommandHandler : IRequestHandler<DeleteHomeCommand, ServiceResult>
{
    private readonly ApplicationDbContext _db;

    public DeleteHomeCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ServiceResult> Handle(DeleteHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes.FirstOrDefaultAsync(h => h.Id == request.HomeId, cancellationToken);

        if (home is null)
            return new NotFoundErrorResult(request.HomeId);

        _db.Homes.Remove(home);

        await _db.SaveChangesAsync(cancellationToken);

        return new SuccessResult();
    }
}
