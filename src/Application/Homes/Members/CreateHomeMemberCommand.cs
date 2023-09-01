using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Homes.Members;

public record CreateHomeMemberCommand(Guid HomeId, MemberProperties Props) : IRequest<ServiceResult<Member>>;

public class AddHomeMemberCommand : IRequestHandler<CreateHomeMemberCommand, ServiceResult<Member>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public AddHomeMemberCommand(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Member>> Handle(CreateHomeMemberCommand request, CancellationToken cancellationToken)
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
