using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Invitations.Requests;

public record AcceptInvitationCommand(Guid InvitationId) : IRequest;

public class AcceptInvitationCommandHandler(ApplicationDbContext db, ICurrentUserService currentUserService)
    : IRequestHandler<AcceptInvitationCommand>
{
    public async Task Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitationId = request.InvitationId;

        var currentUser = currentUserService.CurrentUser!;

        var currentUserEmail = currentUser.GetEmail();

        // todo: test that invitation must be for currently authenticated user
        var invitation = await db.Invitations
            .Include(i => i.Member)
            .FirstOrDefaultAsync(i => i.Id == invitationId && i.Email == currentUserEmail, cancellationToken);

        if (invitation is null)
            throw new ResourceNotFoundException(invitationId);

        if (invitation.Status == DbModels.InvitationStatus.Accepted)
            return; // Nothing to do here

        if (invitation.Status != DbModels.InvitationStatus.Pending)
            throw new ForbiddenException();

        invitation.Status = DbModels.InvitationStatus.Accepted;

        invitation.Member.ContactEmail = currentUserEmail;
        invitation.Member.UserId = currentUser.GetId();
        invitation.Member.Status = DbModels.MemberStatus.Active;

        await db.SaveChangesAsync(cancellationToken);
    }
}
