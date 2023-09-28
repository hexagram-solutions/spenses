using Microsoft.AspNetCore.Authorization;

namespace Spenses.Utilities.Security.Services;

public interface ICurrentUserAuthorizationService
{
    Task<AuthorizationResult> AuthorizeAsync(object? resource, params IAuthorizationRequirement[] requirements);

    Task<AuthorizationResult> AuthorizeAsync(object? resource, string policyName);
}
