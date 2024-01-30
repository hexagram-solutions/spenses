using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spenses.Application.Behaviors;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Features.Identity;
using Spenses.Application.Services.Invitations;
using Spenses.Resources.Communication;
using Spenses.Resources.Relational;
using Spenses.Shared.Models.Invitations;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;
using DbModels = Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Members.Requests;

public record InviteExistingMemberCommand(Guid HomeId, Guid MemberId, InvitationProperties Props)
    : IAuthorizedRequest<Invitation>
{
    public AuthorizationPolicy Policy => Policies.MemberOfHomePolicy(HomeId);
}

public class InviteExistingMemberCommandHandler(
    ApplicationDbContext db,
    IMapper mapper,
    IEmailClient emailClient,
    ICurrentUserService currentUserService,
    IOptions<IdentityEmailOptions> emailOptions,
    InvitationTokenProvider tokenProvider)
    : IRequestHandler<InviteExistingMemberCommand, Invitation>
{
    public async Task<Invitation> Handle(InviteExistingMemberCommand request, CancellationToken cancellationToken)
    {
        var (homeId, memberId, props) = request;

        var member = await db.Members
            .FirstOrDefaultAsync(m => m.HomeId == homeId && m.Id == memberId, cancellationToken);

        if (member is null)
            throw new InvalidRequestException($"Member {memberId} is not a member of home {homeId}");

        if (member.UserId.HasValue)
        {
            throw new InvalidRequestException(new ValidationFailure(nameof(memberId),
                $"Member {memberId} is already associated with a user."));
        }

        member.Status = DbModels.MemberStatus.Invited;

        var existingPendingInvitations = await db.Invitations
            .Where(i => i.MemberId == member.Id && i.Status == DbModels.InvitationStatus.Pending)
            .ToListAsync(cancellationToken);

        existingPendingInvitations.ForEach(i => i.Status = DbModels.InvitationStatus.Cancelled);

        var entry = await db.AddAsync(new DbModels.Invitation
        {
            MemberId = memberId,
            Email = props.Email,
            Status = DbModels.InvitationStatus.Pending,
        }, cancellationToken);

        var invitation = entry.Entity;

        await db.SaveChangesAsync(cancellationToken);

        await SendInvitationEmail(props.Email, homeId, invitation, cancellationToken);

        return mapper.Map<Invitation>(entry.Entity);
    }

    private async Task SendInvitationEmail(string email, Guid homeId, DbModels.Invitation invitation,
        CancellationToken cancellationToken)
    {
        var invitationToken = tokenProvider.ProtectInvitationData(new InvitationData(invitation.Id));

        var acceptInvitationPath = QueryHelpers.AddQueryString(emailOptions.Value.AcceptInvitationPath,
            new Dictionary<string, string?> { { "token", invitationToken } });

        var acceptInvitationUrl = new Uri(new Uri(emailOptions.Value.WebApplicationBaseUrl), acceptInvitationPath);

        var home = await db.Homes.FirstAsync(h => h.Id == homeId, cancellationToken);
        var currentUser = currentUserService.CurrentUser!;

        var invitingUserName = currentUser.GetDisplayName();
        var invitingUserEmail = currentUser.GetEmail();

        var subject = $"[Spenses] {invitingUserName} has invited you to join their home {home.Name}";

        var html = $"{invitingUserName} ({invitingUserEmail}) has invited you to join their home <b>{home.Name}</b> " +
            $"on Spenses. </br></br> <a href=\"{acceptInvitationUrl}\">Accept the invitation</a> to join.";

        var plainText = $"{invitingUserName} ({invitingUserEmail}) has invited you to join their home \"{home.Name}\" " +
            $"on Spenses. \r\n\r\n Follow this link to join: {acceptInvitationUrl}";

        await emailClient.SendEmail(email, subject, html, plainText, cancellationToken);
    }
}
