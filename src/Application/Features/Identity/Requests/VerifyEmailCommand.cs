using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;

namespace Spenses.Application.Features.Identity.Requests;

public record VerifyEmailCommand(VerifyEmailRequest VerifyEmail) : IRequest;

public class VerifyEmailCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<VerifyEmailCommand>
{
    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var (userId, code) = request.VerifyEmail;

        if (await userManager.FindByIdAsync(userId) is not { } user)
        {
            throw new UnauthorizedException();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            throw new UnauthorizedException();
        }

        var result = await userManager.ConfirmEmailAsync(user, code);

        if (!result.Succeeded)
        {
            throw new UnauthorizedException();
        }
    }
}
