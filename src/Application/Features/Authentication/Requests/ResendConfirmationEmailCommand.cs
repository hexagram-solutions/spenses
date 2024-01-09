using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Application.Features.Authentication.Requests;

public record ResendConfirmationEmailCommand(ResendConfirmationEmailRequest Request) : IRequest;

public class ResendConfirmationEmailCommandHandler(
    UserManager<ApplicationUser> userManager, ISender sender)
    : IRequestHandler<ResendConfirmationEmailCommand>
{
    public async Task Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Request.Email) is not { } user)
        {
            // Do nothing
            return;
        }

        await sender.Send(new SendConfirmationEmailCommand(user.Email!), cancellationToken);
    }
}
