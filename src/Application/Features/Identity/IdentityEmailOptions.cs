namespace Spenses.Application.Features.Identity;

public class IdentityEmailOptions
{
    public string WebApplicationBaseUrl { get; set; } = null!;

    public string VerificationPath { get; set; } = null!;

    public string PasswordResetPath { get; set; } = null!;

    public string AcceptInvitationPath { get; set; } = null!;
}
