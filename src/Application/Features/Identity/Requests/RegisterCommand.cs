using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;

namespace Spenses.Application.Features.Identity.Requests;

public record RegisterCommand(RegisterRequest Request) : IRequest<CurrentUser>;

public class RegisterCommandHandler(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore,
    ISender sender)
    : IRequestHandler<RegisterCommand, CurrentUser>
{
    public async Task<CurrentUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (email, password) = request.Request;

        var user = new ApplicationUser();

        //var emailStore = (IUserEmailStore<ApplicationUser>) userStore;
        //await userStore.SetUserNameAsync(user, email, cancellationToken);
        //await emailStore.SetEmailAsync(user, email, cancellationToken);

        await userManager.SetUserNameAsync(user, email);
        await userManager.SetEmailAsync(user, email);
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new InvalidRequestException(result.Errors.Select(e => new ValidationFailure(e.Code, e.Description)));

        await sender.Send(new SendVerificationEmailCommand(email), cancellationToken);

        return new CurrentUser
        {
            Email = email,
            UserName = email,
            EmailVerified = false,
        };
    }
}
