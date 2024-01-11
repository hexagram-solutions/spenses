using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Spenses.Resources.Relational.Models;

namespace Spenses.Application.Features.Identity.Requests;

public record SendVerificationEmailCommand(string Email, bool IsChange = false) : IRequest;

public class SendVerificationEmailCommandHandler(UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender, IOptions<IdentityEmailOptions> emailOptions)
    : IRequestHandler<SendVerificationEmailCommand>
{
    public async Task Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var (email, isChange) = request;

        if (await userManager.FindByEmailAsync(email) is not { } user)
        {
            // Do nothing
            return;
        }

        var code = isChange
            ? await userManager.GenerateChangeEmailTokenAsync(user, email)
            : await userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await userManager.GetUserIdAsync(user);

        var queryParameters = new Dictionary<string, string?> { { "userId", userId }, { "code", code } };

        if (isChange)
            queryParameters.Add("changedEmail", email); // This is validated by the /confirmEmail endpoint on change.

        var emailConfirmationPath = QueryHelpers.AddQueryString(emailOptions.Value.VerificationPath, queryParameters);

        var confirmEmailUrl = new Uri(new Uri(emailOptions.Value.WebApplicationBaseUrl), emailConfirmationPath);

        await emailSender.SendConfirmationLinkAsync(user, email, confirmEmailUrl.ToString());
    }
}