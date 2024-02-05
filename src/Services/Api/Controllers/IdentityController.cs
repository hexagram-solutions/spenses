using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Identity.Requests;
using Spenses.Application.Features.Invitations.Requests;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Models.Me;

namespace Spenses.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("identity")]
[AllowAnonymous]
public class IdentityController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Attempt to authenticate a user using their credentials.
    /// </summary>
    /// <param name="request">The authentication credentials.</param>
    /// <returns>The result of the login attempt.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
    {
        var result = await sender.Send(new LoginCommand(request));

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    /// <summary>
    /// Attempt to authenticate a user using two-factor authentication credentials.
    /// </summary>
    /// <param name="request">The two-factor authentication credentials.</param>
    /// <returns>The result of the authentication attempt.</returns>
    [HttpPost("login-with-2fa")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResult>> TwoFactorLogin(TwoFactorLoginRequest request)
    {
        var result = await sender.Send(new TwoFactorLoginCommand(request));

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>The newly registered user.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CurrentUser>> Register(RegisterRequest request)
    {
        var result = await sender.Send(new RegisterCommand(request));

        return result;
    }

    /// <summary>
    /// Verify a user's email address.
    /// </summary>
    /// <param name="request">The email verification request.</param>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> VerifyEmail(VerifyEmailRequest request)
    {
        await sender.Send(new VerifyEmailCommand(request));

        return Ok();
    }

    /// <summary>
    /// Request a new account verification email.
    /// </summary>
    /// <param name="request">The account verification request.</param>
    [HttpPost("resend-verification-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResendConfirmationEmail(ResendVerificationEmailRequest request)
    {
        await sender.Send(new ResendVerificationEmailCommand(request));

        return Ok();
    }

    /// <summary>
    /// Log the current user out.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Logout()
    {
        await sender.Send(new LogoutCommand());

        return Ok();
    }

    /// <summary>
    /// Request a password reset.
    /// </summary>
    /// <param name="request">The password reset request.</param>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await sender.Send(new ForgotPasswordCommand(request));

        return Ok();
    }

    /// <summary>
    /// Reset a user's password.
    /// </summary>
    /// <param name="request">The password reset request.</param>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await sender.Send(new ResetPasswordCommand(request));

        return Ok();
    }

    /// <summary>
    /// Fetch an invitation to a home.
    /// </summary>
    /// <param name="token">The invitation token.</param>
    /// <returns>The invitation.</returns>
    [HttpGet("invitation")]
    [ApiConventionMethod(typeof(AuthorizedApiConventions), nameof(AuthorizedApiConventions.Get))]
    public async Task<ActionResult<Invitation>> GetInvitation(string token)
    {
        var invitation = await sender.Send(new InvitationQuery(token));

        return Ok(invitation);
    }
}
