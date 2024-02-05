using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Invitations;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Invitations.Requests;

public record AcceptInvitationCommand(Guid InvitationId, Guid InvitedUserId) : IRequest<Invitation>;

public class AcceptInvitationCommandHandler(
    ApplicationDbContext db,
    ICurrentUserService currentUserService,
    IMapper mapper)
    : IRequestHandler<AcceptInvitationCommand, Invitation>
{
    public async Task<Invitation> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var (invitationId, userId) = request;

        var invitation = await db.Invitations
            .Include(i => i.Member)
            .FirstOrDefaultAsync(i => i.Id == invitationId, cancellationToken);

        if (invitation is null)
            throw new ResourceNotFoundException(invitationId);

        if (invitation.Status == DbModels.InvitationStatus.Accepted)
            return mapper.Map<Invitation>(invitation); // Nothing to do here

        if (invitation.Status != DbModels.InvitationStatus.Pending)
            throw new ForbiddenException();

        var authenticatedUser = currentUserService.CurrentUser!;

        if (authenticatedUser.IsAuthenticated() &&
            !string.Equals(invitation.Email, authenticatedUser.GetEmail(), StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ForbiddenException(); // todo: test this
        }

        var invitedUser = await db.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken) ?? throw new ResourceNotFoundException(userId);

        invitation.Status = DbModels.InvitationStatus.Accepted;

        invitation.Member.ContactEmail = invitedUser.Email;
        invitation.Member.UserId = invitedUser.Id;
        invitation.Member.Status = DbModels.MemberStatus.Active;

        await db.SaveChangesAsync(cancellationToken);

        var acceptedInvitation = await db.Invitations
            .ProjectTo<Invitation>(mapper.ConfigurationProvider)
            .FirstAsync(i => i.Id == invitationId, cancellationToken);

        return acceptedInvitation;
    }
}
