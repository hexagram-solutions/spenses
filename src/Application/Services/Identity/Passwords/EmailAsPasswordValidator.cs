using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Application.Services.Identity.Passwords;

public class EmailAsPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user,
        string? password)
    {
        if (string.Equals(user.Email, password, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = IdentityErrors.EmailAsPassword,
                Description = "You cannot use your email as your password."
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}
