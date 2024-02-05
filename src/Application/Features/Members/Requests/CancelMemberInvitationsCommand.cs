using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record CancelMemberInvitationsCommand(Guid HomeId, Guid MemberId) : IAuthorizedRequest
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CancelMemberInvitationsCommandHandler(ApplicationDbContext db)
    : IRequestHandler<CancelMemberInvitationsCommand>
{
    public async Task Handle(CancelMemberInvitationsCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId) = request;

        var member = await db.Members
                .Include(m => m.Invitations.Where(i => i.Status == DbModels.InvitationStatus.Pending))
                .Where(m => m.HomeId == homeId)
                .SingleOrDefaultAsync(m => m.Id == memberId, cancellationToken)
            ?? throw new ResourceNotFoundException(memberId);

        foreach (var pendingInvitation in member.Invitations)
            pendingInvitation.Status = DbModels.InvitationStatus.Cancelled;

        await db.SaveChangesAsync(cancellationToken);
    }
}
