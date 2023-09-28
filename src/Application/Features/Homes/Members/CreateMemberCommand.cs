using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Authorization;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Homes.Members;

public record CreateMemberCommand(Guid HomeId, MemberProperties Props) : IAuthorizedRequest<ServiceResult<Member>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);

    public ServiceResult<Member> OnUnauthorized() => new UnauthorizedErrorResult();
}

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, ServiceResult<Member>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Member>> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, props) = request;

        var home = await _db.Homes.FirstOrDefaultAsync(h => h.Id == homeId, cancellationToken);

        if (home == null)
            return new NotFoundErrorResult(homeId);

        var member = _mapper.Map<DbModels.Member>(props);

        home.Members.Add(member);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Member>(_db.Entry(member).Entity);
    }
}
