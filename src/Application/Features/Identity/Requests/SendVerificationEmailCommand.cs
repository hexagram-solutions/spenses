using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Identity.Requests;

public record SendVerificationEmailCommand(string Email) : IRequest;

public class SendVerificationEmailCommandHandler(UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender, IOptions<IdentityEmailOptions> emailOptions)
    : IRequestHandler<SendVerificationEmailCommand>
{
    public async Task Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            // Do nothing
            return;
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await userManager.GetUserIdAsync(user);

        var queryParameters = new Dictionary<string, string?> { { "userId", userId }, { "code", code } };

        var emailConfirmationPath = QueryHelpers.AddQueryString(emailOptions.Value.VerificationPath, queryParameters);

        var confirmEmailUrl = new Uri(new Uri(emailOptions.Value.WebApplicationBaseUrl), emailConfirmationPath);

        await emailSender.SendConfirmationLinkAsync(user, request.Email, confirmEmailUrl.ToString());
    }
}
