using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Application.Features.Authentication.Requests;

public record TwoFactorLoginCommand(TwoFactorLoginRequest Request) : IRequest<LoginResult>;

public class TwoFactorLoginCommandHandler(SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<TwoFactorLoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(TwoFactorLoginCommand request, CancellationToken cancellationToken)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var result = new SignInResult();

        var (twoFactorCode, twoFactorRecoveryCode, twoFactorRememberClient) = request.Request;

        if (!string.IsNullOrEmpty(twoFactorCode))
        {
            result = await signInManager.TwoFactorAuthenticatorSignInAsync(twoFactorCode, true,
                twoFactorRememberClient);
        }
        else if (!string.IsNullOrEmpty(twoFactorRecoveryCode))
        {
            result = await signInManager.TwoFactorRecoveryCodeSignInAsync(twoFactorRecoveryCode);
        }

        return result.Succeeded
            ? new LoginResult(result.Succeeded)
            : new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsNotAllowed, result.IsLockedOut);
    }
}
