using System.Security.Claims;
using IdentityModel;

namespace Spenses.Utilities.Security;

public static class ApplicationClaimTypes
{
    public const string Email = ClaimTypes.Email;

    public const string EmailVerified = JwtClaimTypes.EmailVerified;

    public const string Identifier = ClaimTypes.NameIdentifier;

    public const string DisplayName = JwtClaimTypes.Name;
}
