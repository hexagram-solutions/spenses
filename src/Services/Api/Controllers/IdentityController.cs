using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spenses.Application.Features.Identity.Requests;
using Spenses.Shared.Models.Identity;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("identity")]
[AllowAnonymous]
public class IdentityController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Authenticate a user using their credentials.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>The result of the login operation.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
    {
        var result = await sender.Send(new LoginCommand(request));

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("login-with-2fa")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResult>> TwoFactorLogin(TwoFactorLoginRequest request)
    {
        var result = await sender.Send(new TwoFactorLoginCommand(request));

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CurrentUser>> Register(RegisterRequest request)
    {
        var result = await sender.Send(new RegisterCommand(request));

        return result;
    }

    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> VerifyEmail(VerifyEmailRequest request)
    {
        await sender.Send(new VerifyEmailCommand(request));

        return Ok();
    }

    [HttpPost("resend-verification-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResendConfirmationEmail(ResendVerificationEmailRequest request)
    {
        await sender.Send(new ResendVerificationEmailCommand(request));

        return Ok();
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Logout()
    {
        await sender.Send(new LogoutCommand());

        return Ok();
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await sender.Send(new ForgotPasswordCommand(request));

        return Ok();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await sender.Send(new ResetPasswordCommand(request));

        return Ok();
    }
}
