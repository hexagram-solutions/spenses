using System.Security.Claims;

namespace Spenses.Application.Services;

public interface ICurrentUserService
{
    ClaimsPrincipal CurrentUser { get; }
}
