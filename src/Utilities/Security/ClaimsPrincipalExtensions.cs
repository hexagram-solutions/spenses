using System.Security.Claims;

namespace Spenses.Utilities.Security;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true;
    }

    private static void EnsureAuthenticated(this ClaimsPrincipal user)
    {
        if (!user.IsAuthenticated())
            throw new InvalidOperationException("Current user is not authenticated.");
    }

    public static Guid GetId(this ClaimsPrincipal user)
    {
        user.EnsureAuthenticated();

        return Guid.Parse(user.FindFirst(ApplicationClaimTypes.Identifier)!.Value);
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ApplicationClaimTypes.Email)!.Value;
    }

    public static string GetDisplayName(this ClaimsPrincipal user)
    {
        user.EnsureAuthenticated();

        return user.FindFirst(ApplicationClaimTypes.DisplayName)!.Value;
    }
}
