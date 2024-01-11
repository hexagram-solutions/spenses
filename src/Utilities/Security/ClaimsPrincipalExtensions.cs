using System.Security.Claims;

namespace Spenses.Utilities.Security;

public static class ClaimsPrincipalExtensions
{
    private static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true;
    }

    private static void EnsureAuthenticated(this ClaimsPrincipal user)
    {
        if (!user.IsAuthenticated())
            throw new InvalidOperationException("Current user is not authenticated.");
    }

    public static string GetId(this ClaimsPrincipal user)
    {
        user.EnsureAuthenticated();

        return user.FindFirst(ApplicationClaimTypes.Identifier)!.Value;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        user.EnsureAuthenticated();

        return user.FindFirst(ApplicationClaimTypes.Email)!.Value;
    }

    public static string GetNickName(this ClaimsPrincipal user)
    {
        user.EnsureAuthenticated();

        return user.FindFirst(ApplicationClaimTypes.NickName)!.Value;
    }
}
