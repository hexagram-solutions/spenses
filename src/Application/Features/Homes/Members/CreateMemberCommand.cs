using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Homes.Members;

public record CreateMemberCommand(Guid HomeId, MemberProperties Props) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, Member>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Member> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await _db.Homes.FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home == null)
            throw new ResourceNotFoundException(homeId);

        var member = _mapper.Map<DbModels.Member>(props);

        home.Members.Add(member);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Member>(_db.Entry(member).Entity);
    }
}
