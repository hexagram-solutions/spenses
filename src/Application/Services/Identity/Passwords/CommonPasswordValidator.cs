using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Relational.Models;

namespace Spenses.Application.Services.Identity.Passwords;

public class CommonPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string? password)
    {
        // TODO

        return Task.FromResult(IdentityResult.Success);
    }
}