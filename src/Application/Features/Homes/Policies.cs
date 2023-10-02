using Microsoft.AspNetCore.Authorization;
using Spenses.Application.Features.Homes.Members;

namespace Spenses.Application.Features.Homes;
public static class Policies
{
    public static AuthorizationPolicy MemberOfHomePolicy(Guid homeId) => new AuthorizationPolicyBuilder()
        .AddRequirements(new HomeMemberRequirement(homeId))
        .Build();
}
