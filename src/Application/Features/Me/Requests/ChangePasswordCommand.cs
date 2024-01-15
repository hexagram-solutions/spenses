using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Features.Me.Requests;

public record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest;

public class ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.GetUserAsync(currentUser.CurrentUser!) is not { } user)
            throw new UnauthorizedException();

        var (currentPassword, newPassword) = request.Request;

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
            throw new InvalidRequestException(result.Errors.Select(e => new ValidationFailure(e.Code, e.Description)));
    }
}
