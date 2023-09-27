using System.Security.Claims;

namespace Spenses.Utilities.Security;

public static class ClaimsPrincipalExtensions
{
    public static string GetId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ApplicationClaimTypes.Identifier)?.Value ??
            user.FindFirst(ApplicationClaimTypes.Subject)!.Value;
    }
}
