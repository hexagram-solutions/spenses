using Spenses.Application.Services;
using System.Security.Claims;

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
