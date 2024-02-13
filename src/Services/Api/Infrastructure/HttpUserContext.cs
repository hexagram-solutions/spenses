using System.Security.Claims;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.Infrastructure;

public class HttpUserContext(IHttpContextAccessor httpContext) : IUserContext
{
    public ClaimsPrincipal CurrentUser => httpContext.HttpContext!.User;
}
