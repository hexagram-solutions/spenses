using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Results;
using Spenses.Application.Models;
using Spenses.Resources.Relational;

namespace Spenses.Application.Homes.Members;

public record UpdateMemberCommand(Guid Id, MemberProperties Props) : IRequest<ServiceResult<Member>>;

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
        var member = await _db.Members.FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (member is null)
            return new NotFoundErrorResult(request.Id);

        _mapper.Map(request.Props, member);

        await _db.SaveChangesAsync(cancellationToken);

        return _mapper.Map<Member>(member);
    }
}
