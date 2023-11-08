using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Models.Members;
using Spenses.Resources.Relational;

namespace Spenses.Application.Features.Members.Requests;

public record ActivateMemberCommand(Guid HomeId, Guid MemberId) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ActivateMemberCommandHandler : IRequestHandler<ActivateMemberCommand, Member>
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ActivateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Member> Handle(ActivateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var homeMember = await _db.Members
            .SingleOrDefaultAsync(m => m.HomeId == homeId && m.Id == memberId, cancellationToken);

        if (homeMember is null)
            throw new ResourceNotFoundException(memberId);

        homeMember.IsActive = true;

        await _db.SaveChangesAsync(cancellationToken);

        var updatedMember = await _db.Members
            .ProjectTo<Member>(_mapper.ConfigurationProvider)
            .FirstAsync(x => x.Id == memberId, cancellationToken);

        return updatedMember;
    }
}
