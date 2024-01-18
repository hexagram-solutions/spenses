using System.Security.Claims;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.Infrastructure;

public class HttpContextCurrentUserService(IHttpContextAccessor httpContext) : ICurrentUserService
{
    public ClaimsPrincipal? CurrentUser => httpContext.HttpContext?.User;
}
