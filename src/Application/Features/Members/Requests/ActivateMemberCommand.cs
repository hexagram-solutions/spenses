using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Members;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record ActivateMemberCommand(Guid HomeId, Guid MemberId) : IAuthorizedRequest<Member>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class ActivateMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<ActivateMemberCommand, Member>
{
    public async Task<Member> Handle(ActivateMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var homeMember = await db.Members
            .SingleOrDefaultAsync(m => m.HomeId == homeId && m.Id == memberId, cancellationToken);

        if (homeMember is null)
            throw new ResourceNotFoundException(memberId);

        homeMember.Status = DbModels.MemberStatus.Active;

        await db.SaveChangesAsync(cancellationToken);

        var updatedMember = await db.Members
            .FirstAsync(x => x.Id == memberId, cancellationToken);

        return mapper.Map<Member>(updatedMember);
    }
}
