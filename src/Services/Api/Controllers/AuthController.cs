using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Models.Authentication;
using Spenses.Resources.Relational.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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

        var (email, password) = request;

        var result = await signInManager.PasswordSignInAsync(email, password, true, true);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsNotAllowed, result.IsLockedOut));
        }

        return Ok(new LoginResult(result.Succeeded));
    }

    [HttpPost("login-with-2fa")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult<LoginResult>> TwoFactorLogin(TwoFactorLoginRequest request)
    {
        signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

        var result = new SignInResult();

        if (!string.IsNullOrEmpty(request.TwoFactorCode))
        {
            result = await signInManager.TwoFactorAuthenticatorSignInAsync(request.TwoFactorCode, true,
                request.TwoFactorRememberClient);
        }
        else if (!string.IsNullOrEmpty(request.TwoFactorRecoveryCode))
        {
            result = await signInManager.TwoFactorRecoveryCodeSignInAsync(request.TwoFactorRecoveryCode);
        }

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status401Unauthorized,
                new LoginResult(result.Succeeded, result.RequiresTwoFactor, result.IsNotAllowed, result.IsLockedOut));
        }

        return Ok(new LoginResult(result.Succeeded));
    }

    [HttpPost("logout")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Post))]
    public async Task<ActionResult> Logout()
    {
        throw new NotImplementedException();
    }

    // confirm email
     
    // resend confirmation

    // forgot password

    // reset password
}

[ApiController]
[ApiVersion("1.0")]
[Route("me")]
public class MeController(SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    // get info

    // post info

    // manage 2fa
}

