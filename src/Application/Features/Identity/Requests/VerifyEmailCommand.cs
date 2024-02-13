using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Utilities;

namespace Spenses.Application.Features.Identity.Requests;

public record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest;

public class VerifyEmailCommandHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IRequestHandler<VerifyEmailCommand>
{
    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var (userId, code, newEmail) = request.Request;

        if (await userManager.FindByIdAsync(userId.ToString()) is not { } user)
            throw new UnauthorizedException();

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            throw new UnauthorizedException();
        }

        var isEmailChange = !string.IsNullOrEmpty(newEmail);

        IdentityResult result;

        if (!isEmailChange)
        {
            result = await userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            result = await userManager.ChangeEmailAsync(user, newEmail!, code);

            if (result.Succeeded)
                result = await userManager.SetUserNameAsync(user, newEmail);
        }

        if (!result.Succeeded)
            throw new UnauthorizedException();

        if (isEmailChange)
        {
            user.AvatarUrl = AvatarHelper.GetGravatarUri(newEmail!).ToString();
            await userManager.UpdateAsync(user);

            await signInManager.SignOutAsync(); // Log the user out to make them re-authenticate with new email
        }
    }
}
