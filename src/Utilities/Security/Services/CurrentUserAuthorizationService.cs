using Microsoft.AspNetCore.Authorization;

namespace Spenses.Utilities.Security.Services;

public class CurrentUserAuthorizationService : ICurrentUserAuthorizationService
{
    private readonly IAuthorizationService _innerAuthorizationService;
    private readonly ICurrentUserService _currentUserService;

    public CurrentUserAuthorizationService(IAuthorizationService innerAuthorizationService,
        ICurrentUserService currentUserService)
    {
        _innerAuthorizationService = innerAuthorizationService;
        _currentUserService = currentUserService;
    }

    public Task<AuthorizationResult> AuthorizeAsync(object? resource, params IAuthorizationRequirement[] requirements)
    {
        return _innerAuthorizationService.AuthorizeAsync(_currentUserService.CurrentUser, resource, requirements);
    }

    public Task<AuthorizationResult> AuthorizeAsync(object? resource, string policyName)
    {
        return _innerAuthorizationService.AuthorizeAsync(_currentUserService.CurrentUser, resource, policyName);
    }
}
