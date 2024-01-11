using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;

namespace Spenses.Application.Features.Identity.Requests;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResult>;

public class LoginCommandHandler(SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var (email, password) = request.Request;

        var result = await signInManager.PasswordSignInAsync(email, password, true, true);

        return result.Succeeded
            ? new LoginResult(result.Succeeded)
            : new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsNotAllowed, result.IsLockedOut);
    }
}
