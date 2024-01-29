using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Application.Services.Invitations;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Invitations.Requests;

public record AcceptInvitationCommand(string Token) : IRequest;

public class AcceptInvitationCommandHandler(ApplicationDbContext db, InvitationTokenProvider tokenProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<AcceptInvitationCommand>
{
    public async Task Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        InvitationData invitationData;

        try
        {
            invitationData = tokenProvider.UnprotectInvitationData(request.Token);
        }
        catch (CryptographicException)
        {
            throw new UnauthorizedException();
        }

        var invitationId = invitationData.InvitationId;

        var invitation = await db.Invitations
            .Include(i => i.Member)
            .FirstOrDefaultAsync(i => i.Id == invitationId, cancellationToken) ?? throw new UnauthorizedException();

        if (invitation.Status == DbModels.InvitationStatus.Accepted)
            return; // Nothing to do here

        if (invitation.Status != DbModels.InvitationStatus.Pending)
            throw new ForbiddenException();

        invitation.Status = DbModels.InvitationStatus.Accepted;

        var currentUser = currentUserService.CurrentUser!;

        invitation.Member.ContactEmail = currentUser.GetEmail();
        invitation.Member.UserId = currentUser.GetId();
        invitation.Member.Status = DbModels.MemberStatus.Active;

        await db.SaveChangesAsync(cancellationToken);
    }
}
