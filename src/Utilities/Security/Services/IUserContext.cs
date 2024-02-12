using System.Security.Claims;

namespace Spenses.Utilities.Security.Services;

public interface IUserContext
{
    ClaimsPrincipal CurrentUser { get; }
}
