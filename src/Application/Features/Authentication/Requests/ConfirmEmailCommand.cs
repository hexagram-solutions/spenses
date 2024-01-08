using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Spenses.Application.Exceptions;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Application.Features.Authentication.Requests;

public record ConfirmEmailCommand(ConfirmEmailRequest ConfirmEmail) : IRequest;

public class ConfirmEmailCommandHandler(UserManager<IdentityUser> userManager) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var (userId, code) = request.ConfirmEmail;

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
