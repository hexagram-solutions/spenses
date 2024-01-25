using System.Security.Claims;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Tools.Setup;

public class SystemCurrentUserService : ICurrentUserService
{
    public static Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public ClaimsPrincipal CurrentUser => new(new ClaimsIdentity(
    [
        new Claim(ApplicationClaimTypes.Identifier, SystemUserId.ToString()),
        new Claim(ApplicationClaimTypes.Email, "system@spenses.money"),
        new Claim(ApplicationClaimTypes.EmailVerified, true.ToString())
    ], nameof(SystemCurrentUserService)));
}
