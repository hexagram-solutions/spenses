using Hexagrams.Extensions.Authentication.OAuth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using IAccessTokenProvider = Hexagrams.Extensions.Authentication.OAuth.IAccessTokenProvider;

namespace Spenses.Client.Web.Infrastructure;

public class WebAssemblyAuthenticationAccessTokenProvider(IAccessTokenProviderAccessor accessor) : IAccessTokenProvider
{
    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var tokenResult =
            await accessor.TokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = scopes });

        tokenResult.TryGetToken(out var accessToken);

        var utcNow = DateTimeOffset.UtcNow;

        return new AccessTokenResponse
        {
            AccessToken = accessToken.Value,
            ExpiresIn = (int) accessToken.Expires.Subtract(utcNow).TotalSeconds,
            Scope = string.Join(' ', accessToken.GrantedScopes)
        };
    }
}
