using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;
using Spenses.Client.Http;
using Spenses.Shared.Models.Authentication;
using Spenses.Utilities.Security;

namespace Spenses.App.Authentication;

public class CookieAuthenticationStateProvider(IAuthApi authApi, IMeApi meApi,
    ILogger<CookieAuthenticationStateProvider> logger)
    : AuthenticationStateProvider, IAuthenticationService
{
    private bool _authenticated;

    public async Task<IdentityResult<LoginResult>> Login(LoginRequest request)
    {
        var result = await authApi.Login(request);

        // The login attempt "failed successfully" (e.g.: credentials were incorrect or the user needs to log in with
        // a second factor)
        if (result.Error is not null && await result.Error.GetContentAsAsync<LoginResult>() is { } loginResult) 
        {
            return new IdentityResult<LoginResult>(loginResult);
        }

        // The login attempt failed for some other reason (e.g.: network error)
        if (result.Error is not null)
        {
            var errors = await result.Error.GetContentAsAsync<ProblemDetails>();

            return new IdentityResult<LoginResult>(null) { Error = errors };
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return new IdentityResult<LoginResult>(result.Content);
    }

    public async Task<IdentityResult> Register(RegisterRequest request)
    {
        var result = await authApi.Register(request);

        if (result.Error is null)
            return new IdentityResult();

        var errors = await result.Error.GetContentAsAsync<ProblemDetails>();

        return new IdentityResult(errors);
    }

    public async Task Logout()
    {
        await authApi.Logout();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();

        return _authenticated;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;

        var userResponse = await meApi.GetMe();

        if (userResponse.Error is not null)
        {
            logger.LogWarning($"Failed to retrieve current user: {userResponse.Error.ReasonPhrase}");

            var unauthenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());

            return new AuthenticationState(unauthenticatedUser);
        }

        var currentUser = userResponse.Content!;

        var claims = new List<Claim>
        {
            new(ApplicationClaimTypes.Name, currentUser.UserName),
            new(ApplicationClaimTypes.Email, currentUser.Email),
            new(ApplicationClaimTypes.EmailVerified, currentUser.EmailVerified.ToString())
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider)));

        _authenticated = true;

        return new AuthenticationState(user);
    }
}
