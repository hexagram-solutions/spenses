using System.Security.Claims;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.Infrastructure;

public class HttpContextCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public HttpContextCurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public ClaimsPrincipal CurrentUser => _httpContext.HttpContext!.User;
}
