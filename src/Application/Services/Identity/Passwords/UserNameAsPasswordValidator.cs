using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Identity;

namespace Spenses.Application.Services.Identity.Passwords;

public class UserNameAsPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user,
        string? password)
    {
        if (string.Equals(user.UserName, password, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = IdentityErrors.Password.UserNameAsPassword,
                Description = "You cannot use your username as your password."
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}
