using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
        var homeMemberUserIds = await _db.Homes
            .Where(h => h.Id == requirement.HomeId)
            .SelectMany(h => h.Members.Select(m => m.UserId))
            .ToListAsync();

        if (homeMemberUserIds.Contains(context.User.GetId()))
            context.Succeed(requirement);
    }
}
