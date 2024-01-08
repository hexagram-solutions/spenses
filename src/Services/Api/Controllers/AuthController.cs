using Asp.Versioning;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spenses.Application.Features.Authentication.Requests;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Authentication;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("auth")]
[AllowAnonymous]
public class AuthController(SignInManager<ApplicationUser> signInManager, ISender sender) : ControllerBase
{
    /// <summary>
    /// Authenticate a user using their credentials.
    /// </summary>
    /// <param name="request">The login credentials</param>
    /// <returns>The new expense category.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
    {
        var result = await sender.Send(new LoginCommand(request));

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("login-with-2fa")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResult>> TwoFactorLogin(TwoFactorLoginRequest request)
    {
        var result = await sender.Send(new TwoFactorLoginCommand(request));

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Logout()
    {
        await sender.Send(new LogoutCommand());

        return Ok();
    }

    // register

    // confirm email
     
    // resend confirmation

    // forgot password

    // reset password
}
