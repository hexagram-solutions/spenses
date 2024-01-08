using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Application.Features.Authentication.Requests;

public record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest;

public class ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender, IOptions<IdentityEmailOptions> emailOptions)
    : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return;

        var code = await userManager.GeneratePasswordResetTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var queryParameters = new Dictionary<string, string?> { { "email", user.Email! }, { "code", code } };

        var passwordResetPath = QueryHelpers.AddQueryString(emailOptions.Value.ConfirmationPath, queryParameters);

        var resetPasswordUrl = new Uri(new Uri(emailOptions.Value.ApplicationBaseUrl), passwordResetPath);

        await emailSender.SendPasswordResetLinkAsync(user, request.Request.Email, resetPasswordUrl.ToString());
    }
}
