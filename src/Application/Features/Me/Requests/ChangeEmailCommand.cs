using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Spenses.Application.Exceptions;
using Spenses.Application.Features.Identity;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Models.Me;
using Spenses.Utilities.Security.Services;

namespace Spenses.Application.Features.Me.Requests;

public record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest;

public class ChangeEmailCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser,
    IEmailSender<ApplicationUser> emailSender, IOptions<IdentityEmailOptions> emailOptions)
    : IRequestHandler<ChangeEmailCommand>
{
    public async Task Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var principal = currentUser.CurrentUser!;

        if (await userManager.GetUserAsync(principal) is not { } user)
            throw new UnauthorizedException();

        var newEmail = request.Request.NewEmail;

        var code = await userManager.GenerateChangeEmailTokenAsync(user, newEmail);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var queryParameters = new Dictionary<string, string?>
        {
            { "userId", user.Id.ToString() },
            { "code", code },
            { "newEmail", newEmail }
        };

        var emailConfirmationPath = QueryHelpers.AddQueryString(emailOptions.Value.VerificationPath, queryParameters);

        var confirmEmailUrl = new Uri(new Uri(emailOptions.Value.WebApplicationBaseUrl), emailConfirmationPath);

        await emailSender.SendConfirmationLinkAsync(user, newEmail, confirmEmailUrl.ToString());
    }
}
