using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Models.Authentication;
using Spenses.Resources.Relational.Models;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("auth")]
[AllowAnonymous]
public class AuthController(SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    /// <summary>
    /// Authenticate a user using their credentials.
    /// </summary>
    /// <param name="request">The login credentials</param>
    /// <returns>The new expense category.</returns>
    [HttpPost("login")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var (email, password, twoFactorCode, twoFactorRecoveryCode) = request;

        var result = await signInManager.PasswordSignInAsync(email, password, true, true);

        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(twoFactorCode))
            {
                result = await signInManager.TwoFactorAuthenticatorSignInAsync(twoFactorCode, true, true);
            }
            else if (!string.IsNullOrEmpty(twoFactorRecoveryCode))
            {
                result = await signInManager.TwoFactorRecoveryCodeSignInAsync(twoFactorRecoveryCode);
            }
        }

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsNotAllowed, result.IsLockedOut));
        }

        return Ok(new LoginResult(result.Succeeded));
    }
}

