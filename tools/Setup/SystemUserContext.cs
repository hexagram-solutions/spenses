using System.Security.Claims;
using Hexagrams.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Spenses.Shared.Common;
using Spenses.Utilities.Security;
using Spenses.Utilities.Security.Services;

namespace Spenses.Tools.Setup;

public class SystemUserContext(IConfiguration config) : IUserContext
{
    public ClaimsPrincipal CurrentUser => new(new ClaimsIdentity(
    [
        new Claim(ApplicationClaimTypes.Identifier, config.Require(ConfigConstants.SpensesTestSystemUserId)),
        new Claim(ApplicationClaimTypes.Email, "system@spenses.money"),
        new Claim(ApplicationClaimTypes.EmailVerified, true.ToString())
    ], nameof(SystemUserContext)));
}
