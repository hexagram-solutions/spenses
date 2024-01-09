using Spenses.App.Identity.Models;
using Spenses.Shared.Models.Authentication;

namespace Spenses.App.Identity;

public interface IAccountManagement
{
    public Task<LoginResult> LoginAsync(LoginRequest request);

    public Task LogoutAsync();

    public Task<FormResult> RegisterAsync(string email, string password);

    public Task<bool> CheckAuthenticatedAsync();
}
