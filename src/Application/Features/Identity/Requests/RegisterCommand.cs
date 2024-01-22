using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Application.Features.Identity.Requests;

public record RegisterCommand(RegisterRequest Request) : IRequest<CurrentUser>;

public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    ISender sender,
    SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<RegisterCommand, CurrentUser>
{
    public async Task<CurrentUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (email, password, displayName) = request.Request;

        var user = new ApplicationUser { DisplayName = displayName };

        await userManager.SetUserNameAsync(user, email);
        await userManager.SetEmailAsync(user, email);
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            await userManager.DeleteAsync(user);

            throw new InvalidRequestException(result.Errors.Select(e => new ValidationFailure(e.Code, e.Description)));
        }

        // Sign-in the user to set the cookie with their email address for the email verification step of the
        // registration flow.
        await signInManager.SignInAsync(user, false);

        await sender.Send(new SendVerificationEmailCommand(email), cancellationToken);

        return new CurrentUser
        {
            Email = user.Email!,
            DisplayName = user.DisplayName,
            EmailVerified = false
        };
    }
}
