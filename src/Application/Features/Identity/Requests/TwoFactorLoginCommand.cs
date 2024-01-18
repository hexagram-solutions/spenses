using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;

namespace Spenses.Application.Features.Identity.Requests;

public record TwoFactorLoginCommand(TwoFactorLoginRequest Request) : IRequest<LoginResult>;

public class TwoFactorLoginCommandHandler(SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<TwoFactorLoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(TwoFactorLoginCommand request, CancellationToken cancellationToken)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var result = new SignInResult();

        var (code, recoveryCode, rememberClient) = request.Request;

        if (!string.IsNullOrEmpty(code))
        {
            result = await signInManager.TwoFactorAuthenticatorSignInAsync(code, true, rememberClient);
        }
        else if (!string.IsNullOrEmpty(recoveryCode))
        {
            result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
        }

        return result.Succeeded
            ? new LoginResult(result.Succeeded)
            : new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsNotAllowed, result.IsLockedOut);
    }
}
