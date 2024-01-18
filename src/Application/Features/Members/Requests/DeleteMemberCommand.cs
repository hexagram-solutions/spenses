using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Common;
using Spenses.Shared.Models.Members;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record DeleteMemberCommand(Guid HomeId, Guid MemberId) : IAuthorizedRequest<DeletionResult<Member>>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class DeleteMemberCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<DeleteMemberCommand, DeletionResult<Member>>
{
    public async Task<DeletionResult<Member>> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await db.Members
            .Where(e => e.Home.Id == homeId)
            .FirstOrDefaultAsync(e => e.Id == memberId, cancellationToken);

        if (member is null)
            throw new ResourceNotFoundException(memberId);

        db.Members.Remove(member);

        try
        {
            await db.SaveChangesAsync(cancellationToken);

            return new DeletionResult<Member>(DeletionType.Deleted, mapper.Map<Member>(member));
        }
        catch (DbUpdateException ex)
            when (ex.Entries.SingleOrDefault(e => (e.Entity as DbModels.Member)?.Id == memberId) is not null)
        {
            // The member has associated entities and can't be deleted, so we'll deactivate the member instead.
            member.IsActive = false;

            // "Undo" the prior removal
            db.Members.Entry(member).State = EntityState.Modified;

            await db.SaveChangesAsync(cancellationToken);

            return new DeletionResult<Member>(DeletionType.Deactivated, mapper.Map<Member>(member));
        }
    }
}
