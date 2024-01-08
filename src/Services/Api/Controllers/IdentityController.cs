using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spenses.Application.Features.Authentication.Requests;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("auth")]
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

    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        await sender.Send(new ConfirmEmailCommand(request));

        return Ok();
    }

    [HttpPost("resend-confirmation-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
    {
        await sender.Send(new ResendConfirmationEmailCommand(request));

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
