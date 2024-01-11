using System.Security.Claims;
using Hexagrams.Extensions.Common.Serialization;
using Microsoft.AspNetCore.Components.Authorization;
using Spenses.Client.Http;
using Spenses.Shared.Models.Identity;
using Spenses.Utilities.Security;

namespace Spenses.Web.Client;

public interface IAuthService
{
    public Task<LoginResult> Login(LoginRequest request);

    public Task<LoginResult> TwoFactorLogin(TwoFactorLoginRequest request);

    public Task Logout();
}

public class CookieAuthenticationStateProvider(IIdentityApi identityApi, IMeApi meApi)
    : AuthenticationStateProvider, IAuthService
{
    private bool _authenticated;

    public async Task<LoginResult> Login(LoginRequest request)
    {
        var result = await identityApi.Login(request);

        if (result.Error is not null)
            return result.Error.Content!.FromJson<LoginResult>()!;

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return result.Content!;
    }

    public async Task<LoginResult> TwoFactorLogin(TwoFactorLoginRequest request)
    {
        var result = await identityApi.TwoFactorLogin(request);

        if (result.Error is not null)
            return result.Error.Content!.FromJson<LoginResult>()!;

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return result.Content!;
    }

    public async Task Logout()
    {
        await identityApi.Logout();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;

        var user = new ClaimsPrincipal(new ClaimsIdentity());

        var userResponse = await meApi.GetMe();

        if (userResponse.Error is not null)
            return new AuthenticationState(user);

        var currentUser = userResponse.Content!;

        var claims = new List<Claim>
        {
            new(ApplicationClaimTypes.UserName, currentUser.UserName),
            new(ApplicationClaimTypes.Email, currentUser.Email),
            new(ApplicationClaimTypes.EmailVerified, currentUser.EmailVerified.ToString())
        };

        user = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider)));

        _authenticated = true;

        return new AuthenticationState(user);
    }
}
