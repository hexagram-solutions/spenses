using System.Security.Claims;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Tools.Setup;

public class SystemCurrentUserService : ICurrentUserService
{
    public const string SystemUserId = "system";

    public ClaimsPrincipal CurrentUser => new(new ClaimsIdentity(new[]
    {
        new Claim(ApplicationClaimTypes.Identifier, SystemUserId),
        new Claim(ApplicationClaimTypes.NickName, "System User"),
        new Claim(ApplicationClaimTypes.Email, "system@spenses.ca"),
        new Claim(ApplicationClaimTypes.EmailVerified, true.ToString())
    }, nameof(SystemCurrentUserService)));
}
