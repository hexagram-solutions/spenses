using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;

namespace Spenses.Application.Features.Identity.Requests;

public record ResendVerificationEmailCommand(ResendVerificationEmailRequest Request) : IRequest;

public class ResendVerificationEmailCommandHandler(
    UserManager<ApplicationUser> userManager, ISender sender)
    : IRequestHandler<ResendVerificationEmailCommand>
{
    public async Task Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Request.Email) is not { } user)
        {
            // Do nothing
            return;
        }

        await sender.Send(new SendVerificationEmailCommand(user.Email!), cancellationToken);
    }
}
