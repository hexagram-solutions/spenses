using System.Text;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Application.Features.Authentication.Requests;

public record ResetPasswordCommand(ResetPasswordRequest Reset) : IRequest;

public class SendPasswordResetEmailCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var (email, resetCode, newPassword) = request.Reset;

        var user = await userManager.FindByEmailAsync(email);

        IdentityResult result;

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }
        else
        {
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetCode));

                result = await userManager.ResetPasswordAsync(user, code, newPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
            }
        }

        if (!result.Succeeded)
        {
            throw new InvalidRequestException(
                result.Errors.Select(e => new ValidationFailure(e.Code, e.Description)));
        }
    }
}
