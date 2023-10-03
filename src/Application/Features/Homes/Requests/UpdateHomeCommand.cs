using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Requests;

public record UpdateHomeCommand(Guid HomeId, HomeProperties Props) : IAuthorizedRequest<Home>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class UpdateHomeCommandHandler : IRequestHandler<UpdateHomeCommand, Home>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateHomeCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Home> Handle(UpdateHomeCommand request, CancellationToken cancellationToken)
    {
        var home = await _db.Homes
            .Include(h => h.Members)
                .ThenInclude(m => m.User)
            .Include(h => h.CreatedBy)
            .Include(h => h.ModifiedBy)
            .FirstOrDefaultAsync(h => h.Id == request.HomeId, cancellationToken);

        if (home is null)
            throw new ResourceNotFoundException(request.HomeId);

        _mapper.Map(request.Props, home);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Home>(home);
    }
}
