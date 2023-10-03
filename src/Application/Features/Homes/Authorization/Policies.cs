using Microsoft.AspNetCore.Authorization;

namespace Spenses.Application.Features.Homes.Authorization;
public static class Policies
{
    public static AuthorizationPolicy MemberOfHomePolicy(Guid homeId) => new AuthorizationPolicyBuilder()
        .AddRequirements(new HomeMemberRequirement(homeId))
        .Build();
}
