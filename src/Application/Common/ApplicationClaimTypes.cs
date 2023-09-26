using System.Security.Claims;
using IdentityModel;

namespace Spenses.Application.Common;

public static class ApplicationClaimTypes
{
    public const string Email = ClaimTypes.Email;

    public const string GivenName = "givenname";

    public const string Issuer = JwtClaimTypes.Issuer;

    public const string Identifier = ClaimTypes.NameIdentifier;

    public const string Name = JwtClaimTypes.Name;

    public const string Permissions = "permissions";

    public const string Role = ClaimTypes.Role;

    public const string Subject = JwtClaimTypes.Subject;

    public const string Surname = "surname";

    public const string UserName = "name";
}
