using System.Security.Claims;
using Spenses.Application.Common;

namespace Spenses.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ApplicationClaimTypes.Identifier)?.Value ??
            user.FindFirst(ApplicationClaimTypes.Subject)!.Value;
    }
}
