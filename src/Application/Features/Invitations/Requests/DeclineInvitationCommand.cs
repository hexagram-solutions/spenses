using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Invitations.Requests;

public record DeclineInvitationCommand(Guid InvitationId) : IRequest;

public class DeclineInvitationCommandHandler(ApplicationDbContext db, ICurrentUserService currentUserService)
    : IRequestHandler<DeclineInvitationCommand>
{
    public async Task Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitationId = request.InvitationId;

        var invitation = await db.Invitations
            .Include(i => i.Member)
            .FirstOrDefaultAsync(i => i.Id == invitationId, cancellationToken);

        if (invitation is null)
            throw new ResourceNotFoundException(invitationId);

        if (invitation.Status == DbModels.InvitationStatus.Declined)
            return; // Nothing to do here

        if (invitation.Status != DbModels.InvitationStatus.Pending)
            throw new ForbiddenException();

        var authenticatedUser = currentUserService.CurrentUser!;

        if (!string.Equals(invitation.Email, authenticatedUser.GetEmail(), StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ForbiddenException(); // todo: test this
        }

        invitation.Status = DbModels.InvitationStatus.Declined;

        await db.SaveChangesAsync(cancellationToken);
    }
}
