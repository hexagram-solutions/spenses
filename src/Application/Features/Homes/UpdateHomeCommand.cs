using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Authorization;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes;

public record UpdateHomeCommand(Guid HomeId, HomeProperties Props) : IAuthorizedRequest<ServiceResult<Home>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult<Home> OnUnauthorized() => new UnauthorizedErrorResult();
}

public class UpdateHomeCommandHandler : IRequestHandler<UpdateHomeCommand, ServiceResult<Home>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateHomeCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Home>> Handle(UpdateHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes
            .Include(h => h.Members)
                .ThenInclude(m => m.User)
            .Include(h => h.CreatedBy)
            .Include(h => h.ModifiedBy)
            .FirstOrDefaultAsync(h => h.Id == request.HomeId, cancellationToken);

        if (home is null)
            return new NotFoundErrorResult(request.HomeId);

        _mapper.Map(request.Props, home);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Home>(home);
    }
}
