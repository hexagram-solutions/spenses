using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Invitations;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record CancelInvitationCommand(Guid HomeId, Guid MemberId, Guid InvitationId) : IAuthorizedRequest<Invitation>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class CancelInvitationCommandHandler(ApplicationDbContext db, IMapper mapper)
    : IRequestHandler<CancelInvitationCommand, Invitation>
{
    public async Task<Invitation> Handle(CancelInvitationCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId, invitationId) = request;

        var member = await db.Members
                .Include(m => m.Invitations.Where(i => i.Id == invitationId))
                .Where(m => m.HomeId == homeId)
                .SingleOrDefaultAsync(m => m.Id == memberId, cancellationToken)
            ?? throw new ResourceNotFoundException(memberId);

        var invitation = member.Invitations.SingleOrDefault() ?? throw new ResourceNotFoundException(invitationId);

        if (invitation.Status == DbModels.InvitationStatus.Accepted)
            throw new InvalidRequestException($"Invitation {invitation.Id} has already been accepted.");

        invitation.Status = DbModels.InvitationStatus.Cancelled;

        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<Invitation>(invitation);
    }
}
