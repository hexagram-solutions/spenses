using Microsoft.AspNetCore.Authorization;
using Spenses.Application.Models;
using Spenses.Utilities.Security;

namespace Spenses.Application.Authorization;

public class HomeMemberRequirement : IAuthorizationRequirement
{
}

public class HomeMemberAuthorizationHandler : AuthorizationHandler<HomeMemberRequirement, Home>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        HomeMemberRequirement requirement, Home resource)
    {
        var homeMemberUserIds = resource.Members
            .Select(m => m.User?.Id);

        if (homeMemberUserIds.Contains(context.User.GetId()))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
