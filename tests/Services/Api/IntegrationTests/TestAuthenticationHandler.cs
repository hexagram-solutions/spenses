using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spenses.Utilities.Security;

namespace Spenses.Api.IntegrationTests;

public class TestAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserIdentifier { get; set; } = null!;

    public string DefaultUserEmail { get; set; } = null!;

    public string DefaultUserNickName { get; set; } = null!;

    public string DefaultUserIssuer { get; set; } = null!;
}

public class TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationHandlerOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
    : AuthenticationHandler<TestAuthenticationHandlerOptions>(options, logger, encoder, clock)
{
    public const string AuthenticationScheme = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var principal = GetClaimsPrincipal();

        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }

    public ClaimsPrincipal GetClaimsPrincipal()
    {
        var claims = new List<Claim> {
            new (ApplicationClaimTypes.Identifier, options.CurrentValue.DefaultUserIdentifier),
            new(ApplicationClaimTypes.NickName, options.CurrentValue.DefaultUserNickName),
            new(ApplicationClaimTypes.Issuer, options.CurrentValue.DefaultUserIssuer),
            new(ApplicationClaimTypes.Email, options.CurrentValue.DefaultUserEmail),
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }
}
