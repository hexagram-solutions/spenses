using System.Security.Claims;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Tools.Setup;

public class SystemCurrentUserService : ICurrentUserService
{
    public static string SystemUserId = "system";

    public ClaimsPrincipal CurrentUser => new(new ClaimsIdentity(new[]
    {
        new Claim(ApplicationClaimTypes.Identifier, SystemUserId),
        new Claim(ApplicationClaimTypes.Name, SystemUserId),
        new Claim(ApplicationClaimTypes.Issuer, "self"),
        new Claim(ApplicationClaimTypes.Email, "system@spenses.ca"),
    }));
}
