using Refit;
using Spenses.Shared.Models.Identity;

namespace Spenses.App.Authentication;

public record IdentityResult(ProblemDetails? Error = null)
{
    public bool Succeeded => Error is null;
}

public record IdentityResult<TResponse>(TResponse? Content) : IdentityResult;

public interface IAuthenticationService
{
    public Task<IdentityResult<LoginResult>> Login(LoginRequest request);

    public Task<IdentityResult> Register(RegisterRequest request);

    public Task<IdentityResult> Logout();

    public Task<bool> CheckAuthenticatedAsync();
}
