using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Invitations.Requests;
using Spenses.Application.Services.Invitations;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;
using Spenses.Shared.Utilities;

namespace Spenses.Application.Features.Identity.Requests;

public record RegisterCommand(RegisterRequest Request) : IRequest<CurrentUser>;

public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext db,
    InvitationTokenProvider invitationTokenProvider,
    ISender sender,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, CurrentUser>
{
    public async Task<CurrentUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (email, password, displayName, invitationToken) = request.Request;

        InvitationData? invitationData = null;

        if (!string.IsNullOrEmpty(invitationToken) &&
            !invitationTokenProvider.TryUnprotectInvitationData(invitationToken, out invitationData))
        {
            throw new InvalidRequestException(new ValidationFailure[]
            {
                new(nameof(RegisterRequest.InvitationToken), "Invitation token was invalid.")
            });
        }

        var user = new ApplicationUser
        {
            DisplayName = displayName,
            AvatarUrl = AvatarHelper.GetGravatarUri(email).ToString()
        };

        await userManager.SetUserNameAsync(user, email);
        await userManager.SetEmailAsync(user, email);
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            await userManager.DeleteAsync(user);

            throw new InvalidRequestException(result.Errors.Select(e => new ValidationFailure(e.Code, e.Description)));
        }

        // TODO: Good candidate for saga pattern; register, send verification email, accept invite
        await sender.Send(new SendVerificationEmailCommand(email), cancellationToken);

        if (invitationData is not null)
        {
            var invitation = await db.Invitations
                .FirstOrDefaultAsync(i => i.Id == invitationData.InvitationId, cancellationToken);

            // Only accept the invitation if it's pending, if it's been cancelled or already accepted, do nothing
            if (invitation is { Status: InvitationStatus.Pending })
            {
                await sender.Send(new AcceptInvitationCommand(invitationData.InvitationId, user.Id), cancellationToken);
            }
            else
            {
                logger.LogWarning(
                    "Attempted to accept invitation {InvitationId} for user {UserId}, but the invitation was not found",
                    invitationData.InvitationId, user.Id);
            }
        }

        return new CurrentUser
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            EmailVerified = false
        };
    }
}
