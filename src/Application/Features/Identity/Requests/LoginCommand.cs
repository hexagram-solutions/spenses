using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;
using Spenses.Utilities.Security;

namespace Spenses.Application.Features.Identity.Requests;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResult>;

public class LoginCommandHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var (email, password) = request.Request;

        if (await userManager.FindByEmailAsync(email) is not { } user)
            return new LoginResult(false);

        var result = await signInManager.CheckPasswordSignInAsync(user, password, true);

        if (!result.Succeeded)
            return new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsNotAllowed, result.IsLockedOut);

        await signInManager.SignInWithClaimsAsync(user, true,
        [
            new Claim(ApplicationClaimTypes.Identifier, user.Id.ToString()),
            new Claim(ApplicationClaimTypes.Email, email),
            new Claim(ApplicationClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower()),
            new Claim(ApplicationClaimTypes.DisplayName, user.DisplayName),
        ]);

        return new LoginResult(result.Succeeded);
    }
}
