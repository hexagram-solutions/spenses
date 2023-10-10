using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Exceptions;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security;

namespace Spenses.Application.Features.Homes.Authorization;

public record HomeMemberRequirement(Guid HomeId) : IAuthorizationRequirement;

public class HomeMemberAuthorizationHandler : AuthorizationHandler<HomeMemberRequirement>
{
    private readonly ApplicationDbContext _db;

    public HomeMemberAuthorizationHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        HomeMemberRequirement requirement)
    {
        var home = await _db.Homes
            .Include(m => m.Members)
            .ThenInclude(m => m.User)
            .SingleOrDefaultAsync(h => h.Id == requirement.HomeId);

        if (home is null)
            throw new ResourceNotFoundException(requirement.HomeId);

        var homeMemberUserIds = home.Members.Select(m => m.UserId);

        if (homeMemberUserIds.Contains(context.User.GetId()))
            context.Succeed(requirement);
    }
}
