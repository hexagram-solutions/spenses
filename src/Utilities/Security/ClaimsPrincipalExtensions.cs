using System.Security.Claims;

namespace Spenses.Utilities.Security;

public static class ClaimsPrincipalExtensions
{
    public static string GetId(this ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            throw new InvalidOperationException("Current user is not authenticated.");

        return user.FindFirst(ApplicationClaimTypes.Identifier)!.Value;
    }
}
