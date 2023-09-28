using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Homes.Members;

public record UpdateMemberCommand(Guid HomeId, Guid MemberId, MemberProperties Props) : IRequest<ServiceResult<Member>>;

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, ServiceResult<Member>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<Member>> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId, props) = request;

        var member = await _db.Members
            .Include(m => m.User)
            .Include(m => m.CreatedBy)
            .Include(m => m.ModifiedBy)
            .Where(m => m.HomeId == homeId)
            .FirstOrDefaultAsync(h => h.Id == memberId, cancellationToken);

        if (member is null)
            return new NotFoundErrorResult(memberId);

        _mapper.Map(props, member);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Member>(member);
    }
}
