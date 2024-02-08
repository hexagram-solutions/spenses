using Microsoft.AspNetCore.Identity;
using Spenses.Resources.Communication;
using Spenses.Resources.Relational.Models;

namespace Spenses.Application.Services.Identity;

public class IdentityEmailSender(IEmailClient emailClient) : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        return emailClient.SendEmail(email, "[Spenses] Verify your account",
            $"Welcome to Spenses!</br></br>You can verify your account by <a href='{confirmationLink}'>clicking here</a>.",
            $"Welcome to Spenses!\r\n\r\nYou use the following link to verify your account:\r\n\r\n{confirmationLink}");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        return emailClient.SendEmail(email, "[Spenses] Reset your password",
            $"A password reset was requested for your Spenses account.</br></br> You can reset your password by <a href='{resetLink}'>clicking here</a>.",
            $"A password reset was requested for your Spenses account.\r\n\r\nYou can use the following link to reset your password:\r\n\r\n{resetLink}");
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        throw new NotSupportedException(
            $"Resetting passwords with codes is not supported. Use {nameof(SendPasswordResetLinkAsync)} instead.");
    }
}
