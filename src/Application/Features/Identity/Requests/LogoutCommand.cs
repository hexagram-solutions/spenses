using MediatR;
using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Identity.Requests;

public record LogoutCommand : IRequest;

public class LogoutCommandHandler(SignInManager<ApplicationUser> signInManager) : IRequestHandler<LogoutCommand>
{
    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return signInManager.SignOutAsync();
    }
}

