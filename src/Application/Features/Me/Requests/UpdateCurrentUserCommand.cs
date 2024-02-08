using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;

namespace Spenses.Application.Features.Me.Requests;

public record UpdateCurrentUserCommand(UserProfileProperties Props) : IRequest<CurrentUser>;

public class UpdateCurrentUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContext,
    IMapper mapper)
    : IRequestHandler<UpdateCurrentUserCommand, CurrentUser>
{
    public async Task<CurrentUser> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var principal = httpContext.HttpContext!.User;

        if (await userManager.GetUserAsync(principal) is not { } currentUser)
            throw new UnauthorizedException();

        mapper.Map(request.Props, currentUser);

        await userManager.UpdateAsync(currentUser);

        return mapper.Map<CurrentUser>(currentUser);
    }
}
