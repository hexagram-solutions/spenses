namespace Spenses.Application.Features.Authentication;

public class IdentityEmailOptions
{
    public string WebApplicationBaseUrl { get; set; } = null!;

    public string ConfirmationPath { get; set; } = null!;

    public string PasswordResetPath { get; set; } = null!;
}
