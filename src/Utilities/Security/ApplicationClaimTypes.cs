using System.Security.Claims;
using IdentityModel;

namespace Spenses.Utilities.Security;

public static class ApplicationClaimTypes
{
    public const string Email = ClaimTypes.Email;

    public const string EmailVerified = JwtClaimTypes.EmailVerified;

    public const string Issuer = JwtClaimTypes.Issuer;

    public const string Identifier = ClaimTypes.NameIdentifier;

    public const string NickName = JwtClaimTypes.NickName;

    public const string Role = ClaimTypes.Role;

    public const string UserName = ClaimTypes.Name;
}
