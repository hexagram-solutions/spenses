using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;

namespace Spenses.Application.Features.Me.Requests;

public record CurrentUserQuery : IRequest<CurrentUser>;

public class CurrentUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContext,
    IMapper mapper)
    : IRequestHandler<CurrentUserQuery, CurrentUser>
{
    public async Task<CurrentUser> Handle(CurrentUserQuery request, CancellationToken cancellationToken)
    {
        var principal = httpContext.HttpContext!.User;

        if (await userManager.GetUserAsync(principal) is not { } currentUser)
            throw new UnauthorizedException();

        return mapper.Map<CurrentUser>(currentUser)!;
    }
}
