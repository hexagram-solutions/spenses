using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Communication;
using Spenses.Resources.Relational.Models;

namespace Spenses.Application.Services;

public class IdentityEmailSender(IEmailClient emailClient) : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        return emailClient.SendEmail(email, "Confirm your email",
            $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.",
            $"Please confirm your account by following this link: {confirmationLink}");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        return emailClient.SendEmail(email, "Reset your password",
            $"Please reset your password by <a href='{resetLink}'>clicking here</a>.",
            $"Please reset your password by following this link: {resetLink}");
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        throw new NotSupportedException(
            $"Resetting passwords with codes is not supported. Use {nameof(SendPasswordResetLinkAsync)} instead.");
    }
}
