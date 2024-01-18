using Microsoft.AspNetCore.Authorization;

namespace Spenses.Utilities.Security.Services;

public class CurrentUserAuthorizationService(IAuthorizationService innerAuthorizationService,
    ICurrentUserService currentUserService) : ICurrentUserAuthorizationService
{
    public Task<AuthorizationResult> AuthorizeAsync(object? resource, params IAuthorizationRequirement[] requirements)
    {
        return innerAuthorizationService.AuthorizeAsync(currentUserService.CurrentUser!, resource, requirements);
    }

    public Task<AuthorizationResult> AuthorizeAsync(object? resource, string policyName)
    {
        return innerAuthorizationService.AuthorizeAsync(currentUserService.CurrentUser!, resource, policyName);
    }
}
