using System.Security.Claims;

namespace Spenses.Utilities.Security.Services;

public interface ICurrentUserService
{
    ClaimsPrincipal? CurrentUser { get; }
}
